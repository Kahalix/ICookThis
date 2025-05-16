using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICookThis.Data;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ICookThis.Modules.Recipes.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly CookThisDbContext _db;
        public RecipeRepository(CookThisDbContext db) => _db = db;

        public async Task<(IEnumerable<Recipe> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount)
        {
            IQueryable<Recipe> query = _db.Recipes;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r => EF.Functions.Like(r.Name, $"%{term}%"));
            }

            if (dishType.HasValue)
            {
                query = query.Where(r => r.DishType == dishType.Value);
            }

            if (statusFilter.HasValue)
                query = query.Where(r => r.Status == statusFilter.Value);

            if (minReviewsCount.HasValue)
                query = query.Where(r => r.ReviewsCount >= minReviewsCount.Value);

            bool desc = sortOrder == SortOrder.Desc;
            switch (sortBy)
            {
                case RecipeSortBy.Name:
                    query = desc
                        ? query.OrderByDescending(r => r.Name)
                        : query.OrderBy(r => r.Name);
                    break;
                case RecipeSortBy.AvgRating:
                    query = desc
                        ? query.OrderByDescending(r => r.AvgRating)
                        : query.OrderBy(r => r.AvgRating);
                    break;
                case RecipeSortBy.AvgPreparationTime:
                    query = desc
                        ? query.OrderByDescending(r => r.AvgPreparationTimeMinutes)
                        : query.OrderBy(r => r.AvgPreparationTimeMinutes);
                    break;
                case RecipeSortBy.ReviewsCount:
                    query = desc
                        ? query.OrderByDescending(r => r.ReviewsCount)
                        : query.OrderBy(r => r.ReviewsCount);
                    break;
                default:
                    query = query.OrderBy(r => r.Name);
                    break;
            }

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        public async Task<(IEnumerable<Recipe> Items, int TotalCount)> GetPagedByUserAsync(
            int ownerId,
            int page, int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder,
            DishType? dishType,
            RecipeStatus? statusFilter,
            int? minReviewsCount)
        {
            IQueryable<Recipe> query = _db.Recipes
                .Where(r => r.UserId == ownerId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r =>
                    EF.Functions.Like(r.Name, $"%{term}%"));
            }

            if (dishType.HasValue)
                query = query.Where(r => r.DishType == dishType.Value);

            if (statusFilter.HasValue)
                query = query.Where(r => r.Status == statusFilter.Value);

            if (minReviewsCount.HasValue)
                query = query.Where(r => r.ReviewsCount >= minReviewsCount.Value);

            bool desc = sortOrder == SortOrder.Desc;
            query = sortBy switch
            {
                RecipeSortBy.Name =>
                    desc ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                RecipeSortBy.AvgRating =>
                    desc ? query.OrderByDescending(r => r.AvgRating) : query.OrderBy(r => r.AvgRating),
                RecipeSortBy.AvgPreparationTime =>
                    desc ? query.OrderByDescending(r => r.AvgPreparationTimeMinutes) : query.OrderBy(r => r.AvgPreparationTimeMinutes),
                RecipeSortBy.ReviewsCount =>
                    desc ? query.OrderByDescending(r => r.ReviewsCount) : query.OrderBy(r => r.ReviewsCount),
                _ => query.OrderBy(r => r.Name),
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

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
    }
    
}