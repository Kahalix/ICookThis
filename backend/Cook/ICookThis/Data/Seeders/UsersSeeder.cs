using System.Threading.Tasks;
using ICookThis.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(25)]
    public class UsersSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Users.AnyAsync()) return;

            var passwordHasher = new PasswordHasher<User>();

            var admin = new User
            {
                UserName = "admin",
                Email = "admin@cookthis.local",
                FirstName = "System",
                LastName = "Administrator",
                TrustFactor = 1.00m,
                ReviewTrustFactor = 1.00m,
                Status = UserStatus.Approved,
                Role = UserRole.Admin
            };
            admin.Password = passwordHasher.HashPassword(admin, "Admin@123");

            var alice = new User
            {
                UserName = "alice",
                Email = "alice@cookthis.local",
                FirstName = "Alice",
                LastName = "Example",
                TrustFactor = 0.75m,
                ReviewTrustFactor = 0.80m,
                Status = UserStatus.Approved,
                Role = UserRole.User
            };
            alice.Password = passwordHasher.HashPassword(alice, "Alice@123");

            var bob = new User
            {
                UserName = "bob",
                Email = "bob@cookthis.local",
                FirstName = "Bob",
                LastName = "Example",
                TrustFactor = 0.60m,
                ReviewTrustFactor = 0.65m,
                Status = UserStatus.Approved,
                Role = UserRole.User
            };
            bob.Password = passwordHasher.HashPassword(bob, "Bob@1234");

            db.Users.AddRange(admin, alice, bob);
            await db.SaveChangesAsync();
        }
    }
}
