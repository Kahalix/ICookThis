using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Recipes.Repositories
{
    public class RecipeIngredientRepository : IRecipeIngredientRepository
    {
        private readonly CookThisDbContext _db;
        public RecipeIngredientRepository(CookThisDbContext db) => _db = db;

        public async Task<IEnumerable<RecipeIngredient>> GetByRecipeAsync(int recipeId) =>
            await _db.RecipeIngredients
                     .Where(ri => ri.RecipeId == recipeId)
                     .ToListAsync();

        public Task<RecipeIngredient?> GetByIdAsync(int id) =>
            _db.RecipeIngredients.FindAsync(id).AsTask();

        public async Task<RecipeIngredient> AddAsync(RecipeIngredient ri)
        {
            _db.RecipeIngredients.Add(ri);
            await _db.SaveChangesAsync();
            return ri;
        }

        public async Task<RecipeIngredient> UpdateAsync(RecipeIngredient ri)
        {
            _db.RecipeIngredients.Update(ri);
            await _db.SaveChangesAsync();
            return ri;
        }

        public async Task DeleteAsync(int id)
        {
            var ri = await _db.RecipeIngredients.FindAsync(id);
            if (ri == null) return;
            _db.RecipeIngredients.Remove(ri);
            await _db.SaveChangesAsync();
        }
    }
}
