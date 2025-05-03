using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Reviews.Repositories;

namespace ICookThis.Modules.Reviews.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepo;

        public ReviewService(
            IReviewRepository repo,
            IMapper mapper,
            IRecipeRepository recipeRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _recipeRepo = recipeRepo;
        }

        public async Task<IEnumerable<ReviewResponse>> GetByRecipeAsync(int recipeId)
        {
            var list = await _repo.GetByRecipeIdAsync(recipeId);
            return _mapper.Map<IEnumerable<ReviewResponse>>(list);
        }

        public async Task<ReviewResponse> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            return _mapper.Map<ReviewResponse>(r);
        }

        public async Task<ReviewResponse> CreateAsync(NewReviewRequest dto)
        {
            var entity = _mapper.Map<Review>(dto);
            var created = await _repo.AddAsync(entity);

            // Po dodaniu recenzji – przelicz statystyki przepisu
            await _recipeRepo.UpdateStatisticsAsync(created.RecipeId);

            return _mapper.Map<ReviewResponse>(created);
        }

        public async Task<ReviewResponse> UpdateAsync(int id, UpdateReviewRequest dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Review with id {id} not found.");
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            // Po edycji recenzji – przelicz statystyki przepisu
            await _recipeRepo.UpdateStatisticsAsync(updated.RecipeId);

            return _mapper.Map<ReviewResponse>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            // Pobierz przed usunięciem, by zachować RecipeId
            var toDelete = await _repo.GetByIdAsync(id);
            if (toDelete == null) return;

            await _repo.DeleteAsync(id);
            await _recipeRepo.UpdateStatisticsAsync(toDelete.RecipeId);
        }
    }
}
