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

        /// <summary> Get paged ingredients with optional search </summary>
        public async Task<(IEnumerable<Ingredient> Items, int TotalCount)> GetPagedAsync(
           int page,
           int pageSize,
           string? search)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            IQueryable<Ingredient> query = _db.Ingredients;

            // opcjonalne filtrowanie po nazwie
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(i => EF.Functions.Like(i.Name, $"%{term}%"));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

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

        public async Task<IEnumerable<Ingredient>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _db.Ingredients
                            .Where(i => ids.Contains(i.Id))
                            .ToListAsync();
        }
    }
}