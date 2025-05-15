using AutoMapper;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Reviews.Repositories;
using ICookThis.Modules.Users.Entities;
using ICookThis.Modules.Users.Repositories;

namespace ICookThis.Modules.Reviews.Services
{
    public class ReviewVoteService : IReviewVoteService
    {
        private readonly IReviewVoteRepository _voteRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public ReviewVoteService(
            IReviewVoteRepository voteRepo,
            IReviewRepository reviewRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _voteRepo = voteRepo;
            _reviewRepo = reviewRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<ReviewVoteDto> VoteAsync(int reviewId, int userId, bool isAgree)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId)
                         ?? throw new KeyNotFoundException($"Review {reviewId} not found");

            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User {userId} not found");
            if (user.Status != UserStatus.Approved)
                throw new UnauthorizedAccessException("Only approved users can vote");

            var existing = await _voteRepo.GetAsync(reviewId, userId);

            if (existing == null)
            {
                existing = new ReviewVote { ReviewId = reviewId, UserId = userId, IsAgree = isAgree };
                await _voteRepo.AddAsync(existing);

                if (isAgree) review.AgreeCount++;
                else review.DisagreeCount++;
            }
            else if (existing.IsAgree == isAgree)
            {
                await _voteRepo.DeleteAsync(existing);
                if (isAgree) review.AgreeCount--;
                else review.DisagreeCount--;
                return _mapper.Map<ReviewVoteDto>(existing);
            }
            else
            {
                if (existing.IsAgree) review.AgreeCount--;
                else review.DisagreeCount--;

                existing.IsAgree = isAgree;
                await _voteRepo.UpdateAsync(existing);

                if (isAgree) review.AgreeCount++;
                else review.DisagreeCount++;
            }

            await _reviewRepo.UpdateAsync(review);

            await RecalculateAuthorTrustFactorAsync(review.UserId);

            return _mapper.Map<ReviewVoteDto>(existing);
        }

        public async Task RemoveVoteAsync(int reviewId, int userId)
        {
            var existing = await _voteRepo.GetAsync(reviewId, userId)
                           ?? throw new KeyNotFoundException("Vote not found");

            var review = await _reviewRepo.GetByIdAsync(reviewId)
                         ?? throw new KeyNotFoundException($"Review {reviewId} not found");

            if (existing.IsAgree) review.AgreeCount--;
            else review.DisagreeCount--;

            await _voteRepo.DeleteAsync(existing);
            await _reviewRepo.UpdateAsync(review);

            await RecalculateAuthorTrustFactorAsync(review.UserId);
        }

        private async Task RecalculateAuthorTrustFactorAsync(int authorId)
        {
            var reviews = await _reviewRepo.GetByUserIdAsync(authorId);
            var ids = reviews.Select(r => r.Id).ToList();
            if (!ids.Any())
            {
                await _userRepo.SetTrustFactorAsync(authorId, 0m);
                return;
            }

            var votes = await _voteRepo.GetByReviewIdsAsync(ids);
            var agree = votes.Count(v => v.IsAgree);
            var disagree = votes.Count() - agree;

            decimal trust = (agree + disagree) > 0
                ? Math.Round((decimal)agree / (agree + disagree), 2)
                : 0m;

            await _userRepo.SetTrustFactorAsync(authorId, trust);
        }
    }
}
