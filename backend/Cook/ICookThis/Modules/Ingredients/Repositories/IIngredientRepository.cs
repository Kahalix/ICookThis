using ICookThis.Modules.Ingredients.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Ingredients.Repositories
{
    public interface IIngredientRepository
    {
        Task<(IEnumerable<Ingredient> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? search);
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(int id);
        Task<Ingredient> AddAsync(Ingredient ingredient);
        Task<Ingredient> UpdateAsync(Ingredient ingredient);
        Task DeleteAsync(int id);
        Task<IEnumerable<Ingredient>> GetByIdsAsync(IEnumerable<int> ids);
    }
}