using ICookThis.Modules.Reviews.Entities;

namespace ICookThis.Modules.Reviews.Repositories
{
    public interface IReviewVoteRepository
    {
        Task<ReviewVote?> GetAsync(int reviewId, int userId);
        Task AddAsync(ReviewVote vote);
        Task UpdateAsync(ReviewVote vote);
        Task DeleteAsync(ReviewVote vote);
        Task<IEnumerable<ReviewVote>> GetByReviewAsync(int reviewId);
        Task<IEnumerable<ReviewVote>> GetByReviewIdsAsync(IEnumerable<int> reviewIds);
    }
}
