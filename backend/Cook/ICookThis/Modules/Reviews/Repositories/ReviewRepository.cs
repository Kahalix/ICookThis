using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ICookThis.Modules.Reviews.Dtos;

namespace ICookThis.Modules.Reviews.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CookThisDbContext _db;
        public ReviewRepository(CookThisDbContext db) => _db = db;

        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedByRecipeAsync(
            int recipeId, int page, int pageSize,
            string? search, ReviewSortBy sortBy,
            SortOrder sortOrder, ReviewStatus? statusFilter)
        {
            var q = _db.Reviews.Where(r => r.RecipeId == recipeId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(r =>
                    EF.Functions.Like(r.Reviewer, $"%{term}%") ||
                    EF.Functions.Like(r.Comment!, $"%{term}%"));
            }

            if (statusFilter.HasValue)
                q = q.Where(r => r.Status == statusFilter.Value);

            bool desc = sortOrder == SortOrder.Desc;
            q = sortBy switch
            {
                ReviewSortBy.Rating =>
                    desc ? q.OrderByDescending(r => r.Rating) : q.OrderBy(r => r.Rating),

                ReviewSortBy.Difficulty =>
                    desc ? q.OrderByDescending(r => r.Difficulty) : q.OrderBy(r => r.Difficulty),

                ReviewSortBy.Helpfulness =>
                    desc
                      ? q.OrderByDescending(r => (decimal)r.AgreeCount / (r.AgreeCount + (decimal)r.DisagreeCount))
                      : q.OrderBy(r => (decimal)r.AgreeCount / (r.AgreeCount + (decimal)r.DisagreeCount)),

                ReviewSortBy.Popularity =>
                    desc
                      ? q.OrderByDescending(r => r.AgreeCount + r.DisagreeCount)
                      : q.OrderBy(r => r.AgreeCount + r.DisagreeCount),

                _ =>
                    desc ? q.OrderByDescending(r => r.CreatedAt) : q.OrderBy(r => r.CreatedAt),
            };

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, total);
        }

        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedByUserAsync(
            int userId, int page, int pageSize,
            string? search, ReviewSortBy sortBy,
            SortOrder sortOrder, ReviewStatus? statusFilter)
        {
            var q = _db.Reviews.Where(r => r.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(r =>
                    EF.Functions.Like(r.Reviewer, $"%{term}%") ||
                    (r.Comment != null && EF.Functions.Like(r.Comment, $"%{term}%")));
            }

            if (statusFilter.HasValue)
                q = q.Where(r => r.Status == statusFilter.Value);

            bool desc = sortOrder == SortOrder.Desc;
            q = sortBy switch
            {
                ReviewSortBy.Rating =>
                    desc ? q.OrderByDescending(r => r.Rating) : q.OrderBy(r => r.Rating),
                ReviewSortBy.Difficulty =>
                    desc ? q.OrderByDescending(r => r.Difficulty) : q.OrderBy(r => r.Difficulty),
                ReviewSortBy.Helpfulness =>
                    desc
                      ? q.OrderByDescending(r => (decimal)r.AgreeCount / (r.AgreeCount + (decimal)r.DisagreeCount))
                      : q.OrderBy(r => (decimal)r.AgreeCount / (r.AgreeCount + (decimal)r.DisagreeCount)),
                ReviewSortBy.Popularity =>
                    desc
                      ? q.OrderByDescending(r => r.AgreeCount + r.DisagreeCount)
                      : q.OrderBy(r => r.AgreeCount + r.DisagreeCount),
                _ => 
                    desc ? q.OrderByDescending(r => r.CreatedAt) : q.OrderBy(r => r.CreatedAt),
            };

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, total);
        }

        public Task<Review?> GetByIdAsync(int id) =>
            _db.Reviews.FindAsync(id).AsTask();

        public async Task<Review> AddAsync(Review review)
        {
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            _db.Reviews.Update(review);
            await _db.SaveChangesAsync();
            return review;
        }

        public async Task DeleteAsync(int id)
        {
            var r = await _db.Reviews.FindAsync(id);
            if (r != null)
            {
                _db.Reviews.Remove(r);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Review>> GetByRecipeIdsAsync(IEnumerable<int> recipeIds)
        {
            if (recipeIds == null || !recipeIds.Any())
                return Array.Empty<Review>();

            return await _db.Reviews
                   .Where(r => recipeIds.Contains(r.RecipeId) && r.Status == ReviewStatus.Approved)
                   .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        {
            return await _db.Reviews
                            .Where(r => r.UserId == userId)
                            .ToListAsync();
        }
    }
}
