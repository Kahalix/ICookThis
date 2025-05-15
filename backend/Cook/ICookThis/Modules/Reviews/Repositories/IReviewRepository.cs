using ICookThis.Shared.Dtos;
using ICookThis.Modules.Reviews.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using ICookThis.Modules.Reviews.Dtos;

namespace ICookThis.Modules.Reviews.Repositories
{
    public interface IReviewRepository
    {
        Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedByRecipeAsync(
            int recipeId,
            int page,
            int pageSize,
            string? search,
            ReviewSortBy sortBy,
            SortOrder sortOrder,
            ReviewStatus? statusFilter);

        Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedByUserAsync(
            int userId,
            int page, int pageSize,
            string? search,
            ReviewSortBy sortBy,
            SortOrder sortOrder,
            ReviewStatus? statusFilter);

        Task<Review?> GetByIdAsync(int id);
        Task<Review> AddAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(int id);
        Task<IEnumerable<Review>> GetByRecipeIdsAsync(IEnumerable<int> recipeIds);
        Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
    }
}
