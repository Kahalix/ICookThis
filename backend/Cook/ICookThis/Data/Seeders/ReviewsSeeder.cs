using System.Linq;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Reviews.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(70)]
    public class ReviewsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Reviews.AnyAsync()) return;

            var pancakeId = db.Recipes.Single(r => r.Name == "Pancakes").Id;
            var mashId = db.Recipes.Single(r => r.Name == "Mashed Potatoes").Id;

            db.Reviews.AddRange(
                new Review
                {
                    RecipeId = pancakeId,
                    Reviewer = "Alice",
                    Difficulty = 2,
                    Recommend = true,
                    Comment = "Very tasty!",
                    Rating = 4.5m,
                    PreparationTimeMinutes = 15
                },
                new Review
                {
                    RecipeId = pancakeId,
                    Reviewer = "Bob",
                    Difficulty = 3,
                    Recommend = false,
                    Comment = "Too sweet.",
                    Rating = 3.0m,
                    PreparationTimeMinutes = 20
                },
                new Review
                {
                    RecipeId = mashId,
                    Reviewer = "Carol",
                    Difficulty = 1,
                    Recommend = true,
                    Comment = "Perfect.",
                    Rating = 5.0m,
                    PreparationTimeMinutes = 30
                }
            );

            await db.SaveChangesAsync();

            await new RecipeRepository(db).UpdateStatisticsAsync(pancakeId);
            await new RecipeRepository(db).UpdateStatisticsAsync(mashId);
        }
    }
}
