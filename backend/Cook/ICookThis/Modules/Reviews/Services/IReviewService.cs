using ICookThis.Modules.Reviews.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Reviews.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponse>> GetByRecipeAsync(int recipeId);
        Task<ReviewResponse> GetByIdAsync(int id);
        Task<ReviewResponse> CreateAsync(NewReviewRequest dto);
        Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto);
        Task DeleteAsync(int id);
    }
}
