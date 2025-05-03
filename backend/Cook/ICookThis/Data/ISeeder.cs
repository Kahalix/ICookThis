using System.Threading.Tasks;

namespace ICookThis.Data
{
    public interface ISeeder
    {
        Task SeedAsync(CookThisDbContext db);
    }
}