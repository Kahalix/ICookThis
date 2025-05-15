using System.Threading.Tasks;
using ICookThis.Modules.Reviews.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(80)]
    public class ReviewVotesSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.ReviewVotes.AnyAsync()) return;
            if (!await db.Reviews.AnyAsync() ||
                !await db.Users.AnyAsync()) return;

            var review1 = await db.Reviews.FirstAsync();
            var review2 = await db.Reviews.Skip(1).FirstAsync();
            var alice = await db.Users.SingleAsync(u => u.UserName == "alice");
            var bob = await db.Users.SingleAsync(u => u.UserName == "bob");

            db.ReviewVotes.AddRange(
                new ReviewVote { ReviewId = review1.Id, UserId = bob.Id, IsAgree = true },
                new ReviewVote { ReviewId = review2.Id, UserId = alice.Id, IsAgree = false }
            );

            await db.SaveChangesAsync();
        }
    }
}
