using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Reviews.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Reviews.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CookThisDbContext _db;
        public ReviewRepository(CookThisDbContext db) => _db = db;

        public async Task<IEnumerable<Review>> GetByRecipeIdAsync(int recipeId) =>
            await _db.Reviews.Where(r => r.RecipeId == recipeId).ToListAsync();

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
            if (r == null) return;
            _db.Reviews.Remove(r);
            await _db.SaveChangesAsync();
        }
    }
}
