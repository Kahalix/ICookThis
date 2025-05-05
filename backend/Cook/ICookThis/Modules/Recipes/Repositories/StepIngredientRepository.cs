using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Recipes.Repositories
{
    public class StepIngredientRepository : IStepIngredientRepository
    {
        private readonly CookThisDbContext _db;
        public StepIngredientRepository(CookThisDbContext db) => _db = db;

        public async Task<IEnumerable<StepIngredient>> GetByStepAsync(int stepId) =>
            await _db.StepIngredients
                     .Where(si => si.InstructionStepId == stepId)
                     .ToListAsync();

        public Task<StepIngredient?> GetByIdAsync(int id) =>
            _db.StepIngredients.FindAsync(id).AsTask();

        public async Task<StepIngredient> AddAsync(StepIngredient si)
        {
            _db.StepIngredients.Add(si);
            await _db.SaveChangesAsync();
            return si;
        }

        public async Task<StepIngredient> UpdateAsync(StepIngredient si)
        {
            _db.StepIngredients.Update(si);
            await _db.SaveChangesAsync();
            return si;
        }

        public async Task DeleteAsync(int id)
        {
            var si = await _db.StepIngredients.FindAsync(id);
            if (si == null) return;
            _db.StepIngredients.Remove(si);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<StepIngredient>> GetByStepIdsAsync(IEnumerable<int> stepIds)
        {
            return await _db.StepIngredients
                            .Where(si => stepIds.Contains(si.InstructionStepId))
                            .ToListAsync();
        }

    }
}
