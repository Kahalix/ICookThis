using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Services
{
    public interface IRecipeService
    {
        //Task<PagedResult<RecipeResponse>> GetPagedAsync(
            //int page,
            //int pageSize,
            //string? search,
            //RecipeSortBy sortBy,
            //SortOrder sortOrder,
            //DishType? dishType,
            //RecipeStatus? statusFilter,
            //int? minReviewsCount,
            //int? userId);
        Task<PagedResult<RecipeListResponse>> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount,
            int? userId);
        Task<PagedResult<RecipeListResponse>> GetByUserAsync(
            int ownerId,
            int page,
            int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount,
            int? currentUserId);

        Task<PagedResult<RecipeListResponse>> GetMyRecipesAsync(
            int page,
            int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount,
            int currentUserId);

        Task<RecipeResponse> ChangeStatusAsync(int recipeId, RecipeStatus newStatus, int userId);
        //        Task<IEnumerable<RecipeResponse>> GetAllAsync();
        Task<RecipeResponse> GetByIdAsync(int id, int? userId, decimal? scale = null);
        Task<RecipeResponse> CreateAsync(NewRecipeRequest dto, int userId);
        Task<RecipeResponse> UpdateAsync(int id, UpdateRecipeRequest dto, int userId);
        Task DeleteAsync(int id);
    }
}
