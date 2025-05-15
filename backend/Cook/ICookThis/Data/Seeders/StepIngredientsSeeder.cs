using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(60)]
    public class StepIngredientsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.StepIngredients.AnyAsync()) return;
            if (!await db.InstructionSteps.AnyAsync() ||
                !await db.Ingredients.AnyAsync()) return;

            var flour = await db.Ingredients.SingleAsync(i => i.Name == "Flour");
            var egg = await db.Ingredients.SingleAsync(i => i.Name == "Egg");
            var milk = await db.Ingredients.SingleAsync(i => i.Name == "Milk");
            var water = await db.Ingredients.SingleAsync(i => i.Name == "Water");

            var pancake = await db.Recipes.SingleAsync(r => r.Name == "Pancakes");
            var mash = await db.Recipes.SingleAsync(r => r.Name == "Mashed Potatoes");

            var step1 = await db.InstructionSteps.SingleAsync(s => s.RecipeId == pancake.Id && s.StepOrder == 1);
            var step2 = await db.InstructionSteps.SingleAsync(s => s.RecipeId == pancake.Id && s.StepOrder == 2);
            var step3 = await db.InstructionSteps.SingleAsync(s => s.RecipeId == mash.Id && s.StepOrder == 1);

            db.StepIngredients.AddRange(
                new StepIngredient { InstructionStepId = step1.Id, IngredientId = flour.Id, Fraction = 1m },
                new StepIngredient { InstructionStepId = step1.Id, IngredientId = egg.Id, Fraction = 1m },
                new StepIngredient { InstructionStepId = step1.Id, IngredientId = milk.Id, Fraction = 1m },
                new StepIngredient { InstructionStepId = step2.Id, IngredientId = flour.Id, Fraction = 1m },
                new StepIngredient { InstructionStepId = step3.Id, IngredientId = water.Id, Fraction = 1m }
            );

            await db.SaveChangesAsync();
        }
    }
}
