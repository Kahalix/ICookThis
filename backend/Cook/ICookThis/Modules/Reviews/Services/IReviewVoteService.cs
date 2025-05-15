using ICookThis.Modules.Reviews.Dtos;

namespace ICookThis.Modules.Reviews.Services
{
    public interface IReviewVoteService
    {
        Task<ReviewVoteDto> VoteAsync(int reviewId, int userId, bool isAgree);
        Task RemoveVoteAsync(int reviewId, int userId);
    }
}
