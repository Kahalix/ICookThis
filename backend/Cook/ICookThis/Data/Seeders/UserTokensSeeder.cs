using System;
using System.Threading.Tasks;
using ICookThis.Modules.Auth.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(26)]
    public class UserTokensSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.UserTokens.AnyAsync()) return;
            if (!await db.Users.AnyAsync()) return;

            var alice = await db.Users.SingleAsync(u => u.UserName == "alice");
            var bob = await db.Users.SingleAsync(u => u.UserName == "bob");

            db.UserTokens.AddRange(
                new UserToken
                {
                    Id = Guid.NewGuid(),
                    UserId = alice.Id,
                    Type = TokenType.EmailConfirmation,
                    Expiry = DateTime.UtcNow.AddDays(7)
                },
                new UserToken
                {
                    Id = Guid.NewGuid(),
                    UserId = bob.Id,
                    Type = TokenType.PasswordReset,
                    Expiry = DateTime.UtcNow.AddHours(1)
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
