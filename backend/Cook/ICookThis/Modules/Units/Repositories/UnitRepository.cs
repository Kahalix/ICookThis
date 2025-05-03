using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Units.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Units.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly CookThisDbContext _db;
        public UnitRepository(CookThisDbContext db) => _db = db;

        public Task<IEnumerable<Unit>> GetAllAsync() =>
            Task.FromResult<IEnumerable<Unit>>(_db.Units);

        public Task<Unit?> GetByIdAsync(int id) =>
            _db.Units.FindAsync(id).AsTask();

        public async Task<Unit> AddAsync(Unit unit)
        {
            _db.Units.Add(unit);
            await _db.SaveChangesAsync();
            return unit;
        }

        public async Task<Unit> UpdateAsync(Unit unit)
        {
            _db.Units.Update(unit);
            await _db.SaveChangesAsync();
            return unit;
        }

        public async Task DeleteAsync(int id)
        {
            var u = await _db.Units.FindAsync(id);
            if (u == null) return;
            _db.Units.Remove(u);
            await _db.SaveChangesAsync();
        }
    }
}
