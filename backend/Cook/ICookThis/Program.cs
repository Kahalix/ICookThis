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

// 0. Prepare builder options with custom WebRootPath
var options = new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
};

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string not found");
builder.Services.AddDbContext<CookThisDbContext>(opt =>
    opt.UseSqlServer(conn));


// 2. Add AutoMapper (scans for profiles in all assemblies)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 3. Register Units module
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitService, UnitService>();

// 4. Register Ingredients module
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();

// 5. Register Recipes module
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

builder.Services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();

builder.Services.AddScoped<IInstructionStepRepository, InstructionStepRepository>();
builder.Services.AddScoped<IInstructionStepService, InstructionStepService>();

builder.Services.AddScoped<IStepIngredientRepository, StepIngredientRepository>();
builder.Services.AddScoped<IStepIngredientService, StepIngredientService>();

// 6. Register Reviews module
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// 6.1 Register Moderation module
builder.Services.AddHttpClient("Moderation", client =>
{
    client.BaseAddress = new Uri("http://moderation:8001");
    client.Timeout = TimeSpan.FromSeconds(5);
});

// 6.2 Register WebRoot wwwroot
builder.WebHost
       .UseWebRoot("wwwroot");


// 7. OpenAPI + Controllers
builder.Services.AddOpenApi();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// 8. Automatyczne migracje (dev only – usunąć EnsureDeleted w prod)
const int maxAttempts = 12;
for (int attempt = 1; attempt <= maxAttempts; attempt++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CookThisDbContext>();
        app.Logger.LogInformation("Próba migracji {Attempt}/{Max}…", attempt, maxAttempts);

        // usuń starą bazę i stwórz od nowa
        //db.Database.EnsureDeleted();

        db.Database.Migrate();
        app.Logger.LogInformation("Migracje zakończone sukcesem.");
        break;
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Migracja nieudana (próba {Attempt}). Za 5s ponawiamy…", attempt);
        if (attempt == maxAttempts)
            throw; // po ostatniej próbie wyrzuć, bo chyba coś naprawdę jest nie tak
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

// 8.1 Seed danych
using var scopeS = app.Services.CreateScope();
var dbS = scopeS.ServiceProvider.GetRequiredService<CookThisDbContext>();
await DbSeeder.SeedAsync(dbS);

// 9. Pipeline

app.UseStaticFiles();

app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();