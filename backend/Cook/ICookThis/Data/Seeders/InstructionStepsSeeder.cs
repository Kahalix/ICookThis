using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(50)]
    public class InstructionStepsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.InstructionSteps.AnyAsync()) return;
            if (!await db.Recipes.AnyAsync()) return;

            var pancake = await db.Recipes.SingleAsync(r => r.Name == "Pancakes");
            var mash = await db.Recipes.SingleAsync(r => r.Name == "Mashed Potatoes");

            db.InstructionSteps.AddRange(
                new InstructionStep
                {
                    RecipeId = pancake.Id,
                    StepOrder = 1,
                    TemplateText = "Mix {Flour}, {Egg} and {Milk}.",
                    Image = "pancakestep1.jpg"
                },
                new InstructionStep
                {
                    RecipeId = pancake.Id,
                    StepOrder = 2,
                    TemplateText = "Fry batter until golden.",
                    Image = "pancakestep2.jpg"
                },
                new InstructionStep
                {
                    RecipeId = mash.Id,
                    StepOrder = 1,
                    TemplateText = "Boil {Water}."
                },
                new InstructionStep
                {
                    RecipeId = mash.Id,
                    StepOrder = 2,
                    TemplateText = "Mash potatoes and season.",
                    Image = "mashstep2.jpg"
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
