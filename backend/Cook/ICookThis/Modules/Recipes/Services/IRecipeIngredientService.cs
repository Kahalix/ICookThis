using ICookThis.Modules.Recipes.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Services
{
    public interface IRecipeIngredientService
    {
        Task<IEnumerable<RecipeIngredientResponse>> GetByRecipeAsync(int recipeId);
        Task<RecipeIngredientResponse> GetByIdAsync(int id);
        Task<RecipeIngredientResponse> CreateAsync(int recipeId, RecipeIngredientRequest dto);
        Task<RecipeIngredientResponse> UpdateAsync(int id, RecipeIngredientRequest dto);
        Task DeleteAsync(int id);
    }
}