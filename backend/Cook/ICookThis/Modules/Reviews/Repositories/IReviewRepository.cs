using ICookThis.Modules.Reviews.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Reviews.Repositories
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetByRecipeIdAsync(int recipeId);
        Task<Review?> GetByIdAsync(int id);
        Task<Review> AddAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(int id);
    }
}
