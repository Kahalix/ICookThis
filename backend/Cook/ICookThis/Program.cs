using Microsoft.EntityFrameworkCore;
using YourApp.Data;

var builder = WebApplication.CreateBuilder(args);
// Konfiguracja DbContext z SQL Server
builder.Services.AddDbContext<CookThisDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Automatyczne stosowanie migracji przy starcie
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CookThisDbContext>();
    app.Logger.LogInformation("Stosowanie migracji...");
    db.Database.Migrate();
    app.Logger.LogInformation("Baza danych przygotowana.");
}

app.Logger.LogInformation("Uruchamianie aplikacji.");
// Tutaj można dodać mapowanie kontrolerów, swagger itp.
app.Run();
