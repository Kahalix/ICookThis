using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Recipes.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Recipes.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly CookThisDbContext _db;
        public RecipeRepository(CookThisDbContext db) => _db = db;

        public Task<IEnumerable<Recipe>> GetAllAsync() =>
            Task.FromResult<IEnumerable<Recipe>>(_db.Recipes);

        public Task<Recipe?> GetByIdAsync(int id) =>
            _db.Recipes.FindAsync(id).AsTask();

        public async Task<Recipe> AddAsync(Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe> UpdateAsync(Recipe recipe)
        {
            _db.Recipes.Update(recipe);
            await _db.SaveChangesAsync();
            return recipe;
        }

        public async Task DeleteAsync(int id)
        {
            var r = await _db.Recipes.FindAsync(id);
            if (r == null) return;
            _db.Recipes.Remove(r);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatisticsAsync(int recipeId)
        {
            var reviews = await _db.Reviews
                                   .Where(r => r.RecipeId == recipeId)
                                   .ToListAsync();

            var recipe = await _db.Recipes.FindAsync(recipeId);
            if (recipe == null) return;

            if (reviews.Any())
            {
                recipe.AvgRating = reviews.Average(r => r.Rating);
                recipe.AvgDifficulty = reviews.Average(r => (decimal)r.Difficulty);
                recipe.RecommendPercentage =
                    reviews.Count(r => r.Recommend) * 100m / reviews.Count;
                recipe.AvgPreparationTimeMinutes =
                    reviews.Average(r => (decimal)r.PreparationTimeMinutes);
            }
            else
            {
                recipe.AvgRating = null;
                recipe.AvgDifficulty = null;
                recipe.RecommendPercentage = null;
                recipe.AvgPreparationTimeMinutes = null;
            }

            _db.Recipes.Update(recipe);
            await _db.SaveChangesAsync();
        }
    }
}
