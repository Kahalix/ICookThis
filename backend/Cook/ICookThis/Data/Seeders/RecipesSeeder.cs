using System.Linq;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(30)]
    public class RecipesSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Recipes.AnyAsync()) return;
            var g = db.Units.Single(u => u.Symbol == "g").Id;
            db.Recipes.AddRange(
                new Recipe
                {
                    Name = "Pancakes",
                    DefaultQty = 500m,
                    DefaultUnitId = g,
                    DishType = DishType.Dessert,
                    Instructions = "Mix and cook"
                },
                new Recipe
                {
                    Name = "Mashed Potatoes",
                    DefaultQty = 1000m,
                    DefaultUnitId = g,
                    DishType = DishType.MainCourse,
                    Instructions = "Boil and mash"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}