using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Repositories;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Repositories;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Units.Repositories;
using ICookThis.Utils;

namespace ICookThis.Modules.Recipes.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRecipeIngredientRepository _riRepo;
        private readonly IInstructionStepRepository _stepRepo;
        private readonly IStepIngredientRepository _siRepo;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IUnitRepository _unitRepo;

        public RecipeService(
            IRecipeRepository recipeRepo,
            IRecipeIngredientRepository riRepo,
            IInstructionStepRepository stepRepo,
            IStepIngredientRepository siRepo,
            IIngredientRepository ingredientRepo,
            IUnitRepository unitRepo,
            IMapper mapper)
        {
            _recipeRepo = recipeRepo;
            _riRepo = riRepo;
            _stepRepo = stepRepo;
            _siRepo = siRepo;
            _ingredientRepo = ingredientRepo;
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeResponse>> GetAllAsync()
        {
            var recipes = await _recipeRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<RecipeResponse>>(recipes);
        }

        public async Task<RecipeResponse> GetByIdAsync(int id, decimal? scale = null)
        {
            var recipe = await _recipeRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Recipe {id} not found");

            // 1. Mapowanie podstawowych pól
            var dto = _mapper.Map<RecipeResponse>(recipe);

            // 2. DefaultUnit
            var defaultUnit = await _unitRepo.GetByIdAsync(recipe.DefaultUnitId);
            dto.DefaultUnit = _mapper.Map<UnitResponse>(defaultUnit);

            // 3. Ingredients
            var ris = await _riRepo.GetByRecipeAsync(id);
            dto.Ingredients = new List<RecipeIngredientResponse>();
            foreach (var ri in ris)
            {
                var ing = await _ingredientRepo.GetByIdAsync(ri.IngredientId);
                var u = await _unitRepo.GetByIdAsync(ri.UnitId);

                dto.Ingredients.Add(new RecipeIngredientResponse
                {
                    Id = ri.Id,
                    RecipeId = ri.RecipeId,
                    Ingredient = _mapper.Map<IngredientResponse>(ing),
                    Qty = ri.Qty,
                    Unit = _mapper.Map<UnitResponse>(u)
                });
            }

            // 4. Steps + StepIngredients
            var steps = await _stepRepo.GetByRecipeAsync(id);
            dto.Steps = new List<InstructionStepResponse>();
            foreach (var step in steps.OrderBy(s => s.StepOrder))
            {
                var stepDto = _mapper.Map<InstructionStepResponse>(step);

                var sis = await _siRepo.GetByStepAsync(step.Id);
                stepDto.StepIngredients = new List<StepIngredientResponse>();
                foreach (var si in sis)
                {
                    var ing = await _ingredientRepo.GetByIdAsync(si.IngredientId);
                    stepDto.StepIngredients.Add(new StepIngredientResponse
                    {
                        Id = si.Id,
                        InstructionStepId = si.InstructionStepId,
                        Ingredient = _mapper.Map<IngredientResponse>(ing),
                        Fraction = si.Fraction
                    });
                }

                // 5. Zastąpienie placeholderów tekstem
                stepDto.Text = step.TemplateText;
                if (scale.HasValue)
                {
                    stepDto.Text = PlaceholderReplacer.Replace(
                        step.TemplateText,
                        dto.Ingredients,
                        stepDto.StepIngredients,
                        scale.Value);
                }

                dto.Steps.Add(stepDto);
            }

            // zastosuj skalowanie do wartości liczbowych
            if (scale.HasValue)
            {
                var s = scale.Value;

                // Skalujemy domyślną ilość porcji
                dto.DefaultQty *= s;

                // Skalujemy wszystkie składniki
                foreach (var ri in dto.Ingredients)
                {
                    ri.Qty *= s;
                }
            }

            return dto;
        }

        public async Task<RecipeResponse> CreateAsync(NewRecipeRequest request)
        {
            // mapowanie głównego przepisu
            var recipeEntity = _mapper.Map<Recipe>(request);
            var created = await _recipeRepo.AddAsync(recipeEntity);

            // składniki przepisu
            var riEntities = request.Ingredients
                .Select(dto =>
                {
                    var e = _mapper.Map<RecipeIngredient>(dto);
                    e.RecipeId = created.Id;
                    return e;
                });

            foreach (var ri in riEntities)
                await _riRepo.AddAsync(ri);

            // kroki instrukcji i ich składniki
            foreach (var stepDto in request.Steps)
            {
                var stepEntity = _mapper.Map<InstructionStep>(stepDto);
                stepEntity.RecipeId = created.Id;
                var addedStep = await _stepRepo.AddAsync(stepEntity);

                var siEntities = (stepDto.StepIngredients
                          ?? Enumerable.Empty<StepIngredientRequest>())
                    .Select(siDto =>
                    {
                        var e = _mapper.Map<StepIngredient>(siDto);
                        e.InstructionStepId = addedStep.Id;
                        return e;
                    });

                foreach (var si in siEntities)
                    await _siRepo.AddAsync(si);
            }

            return await GetByIdAsync(created.Id);
        }

        public async Task<RecipeResponse> UpdateAsync(int id, UpdateRecipeRequest request)
        {
            var existing = await _recipeRepo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Recipe {id} not found");

            _mapper.Map(request, existing);
            await _recipeRepo.UpdateAsync(existing);

            // wymiana składników
            var oldRis = await _riRepo.GetByRecipeAsync(id);
            foreach (var old in oldRis)
                await _riRepo.DeleteAsync(old.Id);

            var newRis = request.Ingredients
                .Select(dto =>
                {
                    var e = _mapper.Map<RecipeIngredient>(dto);
                    e.RecipeId = id;
                    return e;
                });

            foreach (var ri in newRis)
                await _riRepo.AddAsync(ri);

            // wymiana kroków i składników kroków
            var oldSteps = await _stepRepo.GetByRecipeAsync(id);
            foreach (var os in oldSteps)
            {
                var oldSis = await _siRepo.GetByStepAsync(os.Id);
                foreach (var osi in oldSis)
                    await _siRepo.DeleteAsync(osi.Id);

                await _stepRepo.DeleteAsync(os.Id);
            }

            foreach (var stepDto in request.Steps)
            {
                var stepEntity = _mapper.Map<InstructionStep>(stepDto);
                stepEntity.RecipeId = id;
                var addedStep = await _stepRepo.AddAsync(stepEntity);

                var newSis = (stepDto.StepIngredients 
                          ?? Enumerable.Empty<StepIngredientRequest>())
                    .Select(siDto =>
                    {
                        var e = _mapper.Map<StepIngredient>(siDto);
                        e.InstructionStepId = addedStep.Id;
                        return e;
                    });

                foreach (var si in newSis)
                    await _siRepo.AddAsync(si);
            }

            return await GetByIdAsync(id);
        }

        public Task DeleteAsync(int id)
        {
            return _recipeRepo.DeleteAsync(id);
        }
    }
}
