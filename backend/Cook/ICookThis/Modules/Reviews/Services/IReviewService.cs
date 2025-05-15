using ICookThis.Shared.Dtos;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Entities;
using System.Threading.Tasks;

namespace ICookThis.Modules.Reviews.Services
{
    public interface IReviewService
    {
        Task<PagedResult<ReviewResponse>> GetPagedByRecipeAsync(
            int recipeId,
            int page,
            int pageSize,
            string? search,
            ReviewSortBy sortBy,
            SortOrder sortOrder,
            ReviewStatus? statusFilter,
            int? currentUserId);

        Task<PagedResult<ReviewResponse>> GetMyReviewsAsync(
            int page,
            int pageSize,
            string? search,
            ReviewSortBy sortBy,
            SortOrder sortOrder,
            ReviewStatus? statusFilter,
            int currentUserId);

        Task<ReviewResponse> GetByIdAsync(int id, int? currentUserId);
        Task<ReviewResponse> CreateAsync(NewReviewRequest dto, int userId);
        Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto, int currentUserId);
        Task DeleteAsync(int id, int currentUserId);
        Task<ReviewResponse> ChangeStatusAsync(int id, ReviewStatus newStatus, int currentUserId);
    }
}