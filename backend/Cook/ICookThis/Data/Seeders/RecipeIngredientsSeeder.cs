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
            if (await db.RecipeIngredients.AnyAsync()) return;
            if (!await db.Units.AnyAsync() ||
                !await db.Ingredients.AnyAsync() ||
                !await db.Recipes.AnyAsync()) return;

            var flour = await db.Ingredients.SingleAsync(i => i.Name == "Flour");
            var egg = await db.Ingredients.SingleAsync(i => i.Name == "Egg");
            var milk = await db.Ingredients.SingleAsync(i => i.Name == "Milk");
            var sugar = await db.Ingredients.SingleAsync(i => i.Name == "Sugar");
            var water = await db.Ingredients.SingleAsync(i => i.Name == "Water");

            var g = await db.Units.SingleAsync(u => u.Symbol == "g");
            var ml = await db.Units.SingleAsync(u => u.Symbol == "ml");
            var pc = await db.Units.SingleAsync(u => u.Symbol == "pc");

            var pancake = await db.Recipes.SingleAsync(r => r.Name == "Pancakes");
            var mash = await db.Recipes.SingleAsync(r => r.Name == "Mashed Potatoes");

            db.RecipeIngredients.AddRange(
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = flour.Id, Qty = 300m, UnitId = g.Id },
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = egg.Id, Qty = 2m, UnitId = pc.Id },
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = milk.Id, Qty = 200m, UnitId = ml.Id },
                new RecipeIngredient { RecipeId = pancake.Id, IngredientId = sugar.Id, Qty = 50m, UnitId = g.Id },
                new RecipeIngredient { RecipeId = mash.Id, IngredientId = water.Id, Qty = 500m, UnitId = ml.Id }
            );

            await db.SaveChangesAsync();
        }
    }
}
