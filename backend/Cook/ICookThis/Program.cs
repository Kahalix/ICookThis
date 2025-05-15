using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ICookThis.Modules.Recipes.Services;
using ICookThis.Modules.Units.Services;
using ICookThis.Modules.Units.Repositories;
using ICookThis.Modules.Reviews.Services;
using ICookThis.Modules.Reviews.Repositories;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Ingredients.Services;
using ICookThis.Modules.Ingredients.Repositories;
using ICookThis.Data;
using Azure;
using ICookThis.Modules.jwt.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ICookThis.Modules.Users.Repositories;
using ICookThis.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;
using ICookThis.Modules.Users.Services;
using ICookThis.Utils.Email;
using ICookThis.Shared.BackgroundServices;
using ICookThis.Modules.Auth.Services;
using ICookThis.Modules.Auth.Repositories;
using Microsoft.OpenApi.Models;

var options = new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
};

var builder = WebApplication.CreateBuilder(options);

// 1. For Swagger and endpoint explorer
builder.Services.AddEndpointsApiExplorer();

// 2. SwaggerGen (Swashbuckle)
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ICookThis API",
        Version = "v1"
    });

    // JWT Bearer w Swagger UI
    cfg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "W polu wpisz: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 3. Database
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string not found");
builder.Services.AddDbContext<CookThisDbContext>(opt =>
    opt.UseSqlServer(conn));

// 4. AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 5. Register modules (Units, Ingredients, Recipes, Reviews, Users, Auth…)
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitService, UnitService>();

builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();

builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
builder.Services.AddScoped<IInstructionStepRepository, InstructionStepRepository>();
builder.Services.AddScoped<IInstructionStepService, InstructionStepService>();
builder.Services.AddScoped<IStepIngredientRepository, StepIngredientRepository>();
builder.Services.AddScoped<IStepIngredientService, StepIngredientService>();

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IEmailBuilder, EmailBuilder>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<PendingCleanupService>();

// JWT
builder.Services.AddSingleton<IJwtService, JwtService>();
var jwtConfig = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);

builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtConfig["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// Controllers + JSON enumy
builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// CORS
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://www.localhost:5173",
                "http://localhost:5174",
                "http://www.localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// --- Pipeline ---

// Swagger UI (only in dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ICookThis API V1");
        // c.RoutePrefix = string.Empty;
    });
}

// Automatic migrations (dev only)
const int maxAttempts = 12;
for (int attempt = 1; attempt <= maxAttempts; attempt++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CookThisDbContext>();
        app.Logger.LogInformation("Próba migracji {Attempt}/{Max}…", attempt, maxAttempts);

        //db.Database.EnsureDeleted(); // dev only
        db.Database.Migrate();
        app.Logger.LogInformation("Migracje zakończone sukcesem.");
        break;
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Migracja nieudana (próba {Attempt}). Za 5s ponawiamy…", attempt);
        if (attempt == maxAttempts)
            throw;
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

// Data seed
using (var scopeS = app.Services.CreateScope())
{
    var dbS = scopeS.ServiceProvider.GetRequiredService<CookThisDbContext>();
    await DbSeeder.SeedAsync(dbS);
}

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
