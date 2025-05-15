using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(30)]
    public class RecipesSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Recipes.AnyAsync()) return;
            if (!await db.Units.AnyAsync()) return;
            if (!await db.Users.AnyAsync()) return;

            var gId = await db.Units.SingleAsync(u => u.Symbol == "g").ContinueWith(t => t.Result.Id);
            var user = await db.Users.FirstAsync();

            db.Recipes.AddRange(
                new Recipe
                {
                    UserId = user.Id,
                    AddedBy = AddedBy.User,
                    Status = RecipeStatus.Approved,
                    Name = "Pancakes",
                    DefaultQty = 500m,
                    DefaultUnitId = gId,
                    DishType = DishType.Dessert,
                    Description = "Tasty pancakes for you",
                    Image = "pancakes.jpg"
                },
                new Recipe
                {
                    UserId = user.Id+1,
                    AddedBy = AddedBy.User,
                    Status = RecipeStatus.Approved,
                    Name = "Mashed Potatoes",
                    DefaultQty = 1000m,
                    DefaultUnitId = gId,
                    DishType = DishType.MainCourse,
                    Description = "Creamy mashed potatoes with butter and milk",
                    Image = "mashed_potatoes.jpg"
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
