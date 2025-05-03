using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Ingredients.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Ingredients.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly CookThisDbContext _db;
        public IngredientRepository(CookThisDbContext db) => _db = db;

        public Task<IEnumerable<Ingredient>> GetAllAsync() =>
            Task.FromResult<IEnumerable<Ingredient>>(_db.Ingredients);

        public Task<Ingredient?> GetByIdAsync(int id) =>
            _db.Ingredients.FindAsync(id).AsTask();

        public async Task<Ingredient> AddAsync(Ingredient ingredient)
        {
            _db.Ingredients.Add(ingredient);
            await _db.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient> UpdateAsync(Ingredient ingredient)
        {
            _db.Ingredients.Update(ingredient);
            await _db.SaveChangesAsync();
            return ingredient;
        }

        public async Task DeleteAsync(int id)
        {
            var ing = await _db.Ingredients.FindAsync(id);
            if (ing == null) return;
            _db.Ingredients.Remove(ing);
            await _db.SaveChangesAsync();
        }
    }
}
