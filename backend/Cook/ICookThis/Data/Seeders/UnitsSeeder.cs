using System.Threading.Tasks;
using ICookThis.Modules.Units.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Data.Seeders
{
    [SeederOrder(10)]
    public class UnitsSeeder : ISeeder
    {
        public async Task SeedAsync(CookThisDbContext db)
        {
            if (await db.Units.AnyAsync()) return;
            db.Units.AddRange(
                new Unit { Symbol = "g", Type = UnitType.Mass },
                new Unit { Symbol = "kg", Type = UnitType.Mass },
                new Unit { Symbol = "ml", Type = UnitType.Volume },
                new Unit { Symbol = "l", Type = UnitType.Volume },
                new Unit { Symbol = "pc", Type = UnitType.Piece }
            );
            await db.SaveChangesAsync();
        }
    }
}