using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Repositories;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Units.Repositories;

namespace ICookThis.Modules.Recipes.Services
{
    public class RecipeIngredientService : IRecipeIngredientService
    {
        private readonly IRecipeIngredientRepository _repo;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IUnitRepository _unitRepo;
        private readonly IMapper _mapper;

        public RecipeIngredientService(
            IRecipeIngredientRepository repo,
            IIngredientRepository ingredientRepo,
            IUnitRepository unitRepo,
            IMapper mapper)
        {
            _repo = repo;
            _ingredientRepo = ingredientRepo;
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeIngredientResponse>> GetByRecipeAsync(int recipeId)
        {
            var list = await _repo.GetByRecipeAsync(recipeId);

            var responses = new List<RecipeIngredientResponse>();

            foreach (var ri in list)
            {
                var ingredient = await _ingredientRepo.GetByIdAsync(ri.IngredientId);
                var unit = await _unitRepo.GetByIdAsync(ri.UnitId);

                responses.Add(new RecipeIngredientResponse
                {
                    Id = ri.Id,
                    RecipeId = ri.RecipeId,
                    Ingredient = _mapper.Map<IngredientResponse>(ingredient),
                    Qty = ri.Qty,
                    Unit = _mapper.Map<UnitResponse>(unit)
                });
            }

            return responses;
        }

        public async Task<RecipeIngredientResponse> GetByIdAsync(int id)
        {
            var ri = await _repo.GetByIdAsync(id);
            if (ri == null) throw new KeyNotFoundException($"RecipeIngredient with id {id} not found.");

            var ingredient = await _ingredientRepo.GetByIdAsync(ri.IngredientId);
            var unit = await _unitRepo.GetByIdAsync(ri.UnitId);

            return new RecipeIngredientResponse
            {
                Id = ri.Id,
                RecipeId = ri.RecipeId,
                Ingredient = _mapper.Map<IngredientResponse>(ingredient),
                Qty = ri.Qty,
                Unit = _mapper.Map<UnitResponse>(unit)
            };
        }


        public async Task<RecipeIngredientResponse> CreateAsync(int recipeId, RecipeIngredientRequest dto)
        {
            var entity = _mapper.Map<RecipeIngredient>(dto);
            entity.RecipeId = recipeId;
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<RecipeIngredientResponse>(created);
        }

        public async Task<RecipeIngredientResponse> UpdateAsync(int id, RecipeIngredientRequest dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"RecipeIngredient with id {id} not found.");
            _mapper.Map(dto, existing);

            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<RecipeIngredientResponse>(updated);
        }

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}