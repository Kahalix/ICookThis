using System.Threading.Tasks;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Recipes.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(70)]
    public class ReviewsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Reviews.AnyAsync()) return;
            if (!await db.Recipes.AnyAsync() ||
                !await db.Users.AnyAsync()) return;

            var pancake = await db.Recipes.SingleAsync(r => r.Name == "Pancakes");
            var mash = await db.Recipes.SingleAsync(r => r.Name == "Mashed Potatoes");
            var alice = await db.Users.SingleAsync(u => u.UserName == "alice");
            var bob = await db.Users.SingleAsync(u => u.UserName == "bob");

            db.Reviews.AddRange(
                new Review
                {
                    RecipeId = pancake.Id,
                    UserId = alice.Id,
                    Reviewer = alice.UserName,
                    Difficulty = 2,
                    Recommend = true,
                    Comment = "Very tasty!",
                    Rating = 4.5m,
                    PreparationTimeMinutes = 15
                },
                new Review
                {
                    RecipeId = pancake.Id,
                    UserId = bob.Id,
                    Reviewer = bob.UserName,
                    Difficulty = 3,
                    Recommend = false,
                    Comment = "Too sweet.",
                    Rating = 3.0m,
                    PreparationTimeMinutes = 20
                },
                new Review
                {
                    RecipeId = mash.Id,
                    UserId = alice.Id,
                    Reviewer = alice.UserName,
                    Difficulty = 1,
                    Recommend = true,
                    Comment = "Perfect.",
                    Rating = 5.0m,
                    PreparationTimeMinutes = 30
                }
            );

            await db.SaveChangesAsync();

        }
    }
}
