using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Repositories
{
    public interface IRecipeRepository
    {
        /// <summary>Gets paged recipes, with optional sorting and name search.</summary>
        Task<(IEnumerable<Recipe> Items, int TotalCount)> GetPagedAsync(
           int page,
           int pageSize,
           string? search,
           RecipeSortBy sortBy,
           SortOrder sortOrder,
           DishType? dishType,
           RecipeStatus? statusFilter,
           int? minReviewsCount);

        Task<(IEnumerable<Recipe> Items, int TotalCount)> GetPagedByUserAsync(
            int ownerId,
            int page, int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount);

        Task<IEnumerable<Recipe>> GetAllAsync();
        Task<Recipe?> GetByIdAsync(int id);
        Task<Recipe> AddAsync(Recipe recipe);
        Task<Recipe> UpdateAsync(Recipe recipe);
        Task DeleteAsync(int id);

    }
}