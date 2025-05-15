using System.Threading.Tasks;
using ICookThis.Modules.Ingredients.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(20)]
    public class IngredientsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Ingredients.AnyAsync()) return;

            db.Ingredients.AddRange(
                new Ingredient { Name = "Flour" },
                new Ingredient { Name = "Water" },
                new Ingredient { Name = "Egg" },
                new Ingredient { Name = "Milk" },
                new Ingredient { Name = "Sugar" }
            );

            await db.SaveChangesAsync();
        }
    }
}
