using ICookThis.Data;
using ICookThis.Modules.Reviews.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Reviews.Repositories
{
    public class ReviewVoteRepository : IReviewVoteRepository
    {
        private readonly CookThisDbContext _db;
        public ReviewVoteRepository(CookThisDbContext db) => _db = db;

        public Task<ReviewVote?> GetAsync(int reviewId, int userId) =>
            _db.Set<ReviewVote>()
               .FindAsync(reviewId, userId)
               .AsTask();

        public async Task AddAsync(ReviewVote vote)
        {
            _db.Set<ReviewVote>().Add(vote);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ReviewVote vote)
        {
            _db.Set<ReviewVote>().Update(vote);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(ReviewVote vote)
        {
            _db.Set<ReviewVote>().Remove(vote);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewVote>> GetByReviewAsync(int reviewId)
        {
            return await _db.Set<ReviewVote>()
                            .Where(v => v.ReviewId == reviewId)
                            .ToListAsync();
        }

        public async Task<IEnumerable<ReviewVote>> GetByReviewIdsAsync(IEnumerable<int> reviewIds)
        {
            return await _db.Set<ReviewVote>()
                            .Where(v => reviewIds.Contains(v.ReviewId))
                            .ToListAsync();
        }
    }
}
