using System.Linq;
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

            var pancakeId = db.Recipes.Single(r => r.Name == "Pancakes").Id;
            var mashId = db.Recipes.Single(r => r.Name == "Mashed Potatoes").Id;

            db.InstructionSteps.AddRange(
                new InstructionStep
                {
                    RecipeId = pancakeId,
                    StepOrder = 1,
                    TemplateText = "Mix {Flour}, {Egg} and {Milk}."
                },
                new InstructionStep
                {
                    RecipeId = pancakeId,
                    StepOrder = 2,
                    TemplateText = "Fry batter until golden."
                },
                new InstructionStep
                {
                    RecipeId = mashId,
                    StepOrder = 1,
                    TemplateText = "Boil {Water}."
                },
                new InstructionStep
                {
                    RecipeId = mashId,
                    StepOrder = 2,
                    TemplateText = "Mash potatoes and season."
                }
            );

            await db.SaveChangesAsync();
        }
    }
}