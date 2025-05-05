using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Recipes.Repositories
{
    public class InstructionStepRepository : IInstructionStepRepository
    {
        private readonly CookThisDbContext _db;
        public InstructionStepRepository(CookThisDbContext db) => _db = db;

        public async Task<IEnumerable<InstructionStep>> GetByRecipeAsync(int recipeId) =>
            await _db.InstructionSteps
                     .Where(s => s.RecipeId == recipeId)
                     .OrderBy(s => s.StepOrder)
                     .ToListAsync();

        public Task<InstructionStep?> GetByIdAsync(int id) =>
            _db.InstructionSteps.FindAsync(id).AsTask();

        public async Task<InstructionStep> AddAsync(InstructionStep step)
        {
            _db.InstructionSteps.Add(step);
            await _db.SaveChangesAsync();
            return step;
        }

        public async Task<InstructionStep> UpdateAsync(InstructionStep step)
        {
            _db.InstructionSteps.Update(step);
            await _db.SaveChangesAsync();
            return step;
        }

        public async Task DeleteAsync(int id)
        {
            var s = await _db.InstructionSteps.FindAsync(id);
            if (s == null) return;
            _db.InstructionSteps.Remove(s);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<InstructionStep>> GetByRecipeIdsAsync(IEnumerable<int> recipeIds)
        {
            return await _db.InstructionSteps
                            .Where(s => recipeIds.Contains(s.RecipeId))
                            .ToListAsync();
        }
    }
}
