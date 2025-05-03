using System.Linq;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(40)]
    public class RecipeIngredientsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.RecipeIngredients.AnyAsync())
                return;

            // 1) Assured that there are already Units, Ingredients and Recipes
            if (!await db.Units.AnyAsync() ||
                !await db.Ingredients.AnyAsync() ||
                !await db.Recipes.AnyAsync())
            {
                // Not all tables are seeded yet, so we can't proceed
                return;
            }

            // 2) Get the required items from the database
            var flour = db.Ingredients.FirstOrDefault(i => i.Name == "Flour");
            var egg = db.Ingredients.FirstOrDefault(i => i.Name == "Egg");
            var milk = db.Ingredients.FirstOrDefault(i => i.Name == "Milk");
            var sugar = db.Ingredients.FirstOrDefault(i => i.Name == "Sugar");
            var water = db.Ingredients.FirstOrDefault(i => i.Name == "Water");

            var g = db.Units.FirstOrDefault(u => u.Symbol == "g");
            var ml = db.Units.FirstOrDefault(u => u.Symbol == "ml");
            var pc = db.Units.FirstOrDefault(u => u.Symbol == "pc");

            var pancake = db.Recipes.FirstOrDefault(r => r.Name == "Pancakes");
            var mash = db.Recipes.FirstOrDefault(r => r.Name == "Mashed Potatoes");

            // 3) Check if all required items are present
            if (flour == null || egg == null || milk == null || sugar == null || water == null
             || g == null || ml == null || pc == null
             || pancake == null || mash == null)
            {
                // If any of the required items are missing, we can't proceed
                return;
            }

            // 4) pancakes
            db.RecipeIngredients.AddRange(
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = flour.Id, Qty = 300m, UnitId = g.Id },
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = egg.Id, Qty = 2m, UnitId = pc.Id },
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = milk.Id, Qty = 200m, UnitId = ml.Id },
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = sugar.Id, Qty = 50m, UnitId = g.Id },
            // 5) mashed potatoes
                new RecipeIngredient { RecipeId = mash.Id, IngredientId = water.Id, Qty = 500m, UnitId = ml.Id }
            );

            await db.SaveChangesAsync();
        }
    }
}
