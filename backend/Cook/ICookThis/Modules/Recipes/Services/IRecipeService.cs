using ICookThis.Modules.Recipes.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Services
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeResponse>> GetAllAsync();
        Task<RecipeResponse> GetByIdAsync(int id, decimal? scale = null);
        Task<RecipeResponse> CreateAsync(NewRecipeRequest dto);
        Task<RecipeResponse> UpdateAsync(int id, UpdateRecipeRequest dto);
        Task DeleteAsync(int id);
    }
}
