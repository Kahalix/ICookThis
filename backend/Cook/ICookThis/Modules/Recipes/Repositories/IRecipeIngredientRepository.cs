using ICookThis.Modules.Recipes.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Repositories
{
    public interface IRecipeIngredientRepository
    {
        Task<IEnumerable<RecipeIngredient>> GetByRecipeAsync(int recipeId);
        Task<RecipeIngredient?> GetByIdAsync(int id);
        Task<RecipeIngredient> AddAsync(RecipeIngredient ri);
        Task<RecipeIngredient> UpdateAsync(RecipeIngredient ri);
        Task DeleteAsync(int id);
        Task<IEnumerable<RecipeIngredient>> GetByRecipeIdsAsync(IEnumerable<int> recipeIds);
    }
}
