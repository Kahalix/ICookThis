// Data/Seeders/StepIngredientsSeeder.cs
using System.Linq;
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
            if (await db.StepIngredients.AnyAsync())
                return;

            // 1) Assure that we have steps and ingredients
            if (!await db.InstructionSteps.AnyAsync() ||
                !await db.Ingredients.AnyAsync())
            {
                // missing steps or ingredients
                return;
            }

            // 2) Get IDs of ingredients
            var flour = db.Ingredients.FirstOrDefault(i => i.Name == "Flour");
            var egg = db.Ingredients.FirstOrDefault(i => i.Name == "Egg");
            var milk = db.Ingredients.FirstOrDefault(i => i.Name == "Milk");
            var water = db.Ingredients.FirstOrDefault(i => i.Name == "Water");

            // 3) Get ID of recipes and steps
            var pancakeId = db.Recipes.FirstOrDefault(r => r.Name == "Pancakes")?.Id;
            var mashId = db.Recipes.FirstOrDefault(r => r.Name == "Mashed Potatoes")?.Id;

            var step1 = db.InstructionSteps
                          .FirstOrDefault(s => s.RecipeId == pancakeId && s.StepOrder == 1);
            var step2 = db.InstructionSteps
                          .FirstOrDefault(s => s.RecipeId == pancakeId && s.StepOrder == 2);
            var step3 = db.InstructionSteps
                          .FirstOrDefault(s => s.RecipeId == mashId && s.StepOrder == 1);
            // (step mash 2 doesn't need ingredients)

            // 4) Check if all ingredients and steps are available
            if (flour == null || egg == null || milk == null || water == null
             || step1 == null || step2 == null || step3 == null)
            {
                // if any ingredient or step is missing, we can't proceed
                return;
            }

            // 5) Add StepIngredients
            db.StepIngredients.AddRange(
                // Pancakes step 1: mix
                new StepIngredient { InstructionStepId = step1.Id, IngredientId = flour.Id, Fraction = 1m },
                new StepIngredient { InstructionStepId = step1.Id, IngredientId = egg.Id, Fraction = 1m },
                new StepIngredient { InstructionStepId = step1.Id, IngredientId = milk.Id, Fraction = 1m },

                // Pancakes step 2: fry
                new StepIngredient { InstructionStepId = step2.Id, IngredientId = flour.Id, Fraction = 1m },

                // Mashed Potatoes step 1: boil water
                new StepIngredient { InstructionStepId = step3.Id, IngredientId = water.Id, Fraction = 1m }
            );

            await db.SaveChangesAsync();
        }
    }
}
