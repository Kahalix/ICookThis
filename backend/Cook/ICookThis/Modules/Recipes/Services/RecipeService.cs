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
using ICookThis.Shared.Dtos;
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
        private readonly IWebHostEnvironment _env;

        public RecipeService(
            IRecipeRepository recipeRepo,
            IRecipeIngredientRepository riRepo,
            IInstructionStepRepository stepRepo,
            IStepIngredientRepository siRepo,
            IIngredientRepository ingredientRepo,
            IUnitRepository unitRepo,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _recipeRepo = recipeRepo;
            _riRepo = riRepo;
            _stepRepo = stepRepo;
            _siRepo = siRepo;
            _ingredientRepo = ingredientRepo;
            _unitRepo = unitRepo;
            _mapper = mapper;
            _env = env;
        }

        public async Task<PagedResult<RecipeResponse>> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            RecipeSortBy sortBy,
            SortOrder sortOrder)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            // 1) pobierz przepisów z repo (z filtrem, sortowaniem, paginacją)
            var (recipes, total) = await _recipeRepo.GetPagedAsync(
                page, pageSize, search, sortBy, sortOrder);

            // 2) batch-load składników
            var recipeIds = recipes.Select(r => r.Id).ToList();
            var allRis = (await _riRepo.GetByRecipeIdsAsync(recipeIds)).ToList();
            var ingredientIds = allRis.Select(ri => ri.IngredientId).Distinct();
            var unitIds = allRis.Select(ri => ri.UnitId).Distinct();

            var allIngredients = (await _ingredientRepo.GetByIdsAsync(ingredientIds))
                                     .ToDictionary(i => i.Id);
            var allUnits = (await _unitRepo.GetByIdsAsync(unitIds))
                                     .ToDictionary(u => u.Id);

            // 3) budujemy DTO
            var items = new List<RecipeResponse>(recipes.Count());
            foreach (var recipe in recipes)
            {
                var dto = _mapper.Map<RecipeResponse>(recipe);

                // DefaultUnit
                var du = await _unitRepo.GetByIdAsync(recipe.DefaultUnitId);
                dto.DefaultUnit = _mapper.Map<UnitResponse>(du);

                // Ingredients
                var risForThis = allRis.Where(ri => ri.RecipeId == recipe.Id);
                dto.Ingredients = risForThis.Select(ri => new RecipeIngredientResponse
                {
                    Id = ri.Id,
                    RecipeId = ri.RecipeId,
                    Ingredient = _mapper.Map<IngredientResponse>(allIngredients[ri.IngredientId]),
                    Qty = ri.Qty,
                    Unit = _mapper.Map<UnitResponse>(allUnits[ri.UnitId])
                }).ToList();

                items.Add(dto);
            }

            // 4) zwróć PagedResult
            return new PagedResult<RecipeResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = items,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Search = search
            };
        }
        public async Task<IEnumerable<RecipeResponse>> GetAllAsync()
        {
            var recipes = await _recipeRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<RecipeResponse>>(recipes);
        }

        public async Task<RecipeResponse> GetByIdAsync(int id, decimal? scale = null)
        {
            // 1) Pobierz główną encję
            var recipe = await _recipeRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Recipe {id} not found");

            // 2) Mapowanie podstawowych pól
            var dto = _mapper.Map<RecipeResponse>(recipe);

            // 3) DefaultUnit (tylko jedno zapytanie)
            var defaultUnit = await _unitRepo.GetByIdAsync(recipe.DefaultUnitId);
            dto.DefaultUnit = _mapper.Map<UnitResponse>(defaultUnit);

            // 4) Batch-load RecipeIngredients
            var ris = (await _riRepo.GetByRecipeIdsAsync(new[] { id })).ToList();
            // 4a) Batch-load powiązanych Ingredient i Unit
            var ingredientIds = ris.Select(ri => ri.IngredientId).Distinct();
            var unitIds = ris.Select(ri => ri.UnitId).Distinct();

            var allIngredients = (await _ingredientRepo.GetByIdsAsync(ingredientIds))
                                 .ToDictionary(i => i.Id);
            var allUnits = (await _unitRepo.GetByIdsAsync(unitIds))
                                 .ToDictionary(u => u.Id);

            dto.Ingredients = ris.Select(ri => new RecipeIngredientResponse
            {
                Id = ri.Id,
                RecipeId = ri.RecipeId,
                Ingredient = _mapper.Map<IngredientResponse>(allIngredients[ri.IngredientId]),
                Qty = ri.Qty,
                Unit = _mapper.Map<UnitResponse>(allUnits[ri.UnitId])
            }).ToList();

            // 5) Batch-load InstructionSteps
            var steps = (await _stepRepo.GetByRecipeIdsAsync(new[] { id }))
                        .OrderBy(s => s.StepOrder)
                        .ToList();
            var stepIds = steps.Select(s => s.Id).ToList();

            // 5a) Batch-load StepIngredients
            var sis = (await _siRepo.GetByStepIdsAsync(stepIds)).ToList();
            var siIngredientIds = sis.Select(si => si.IngredientId).Distinct();
            var allSiIngredients = (await _ingredientRepo.GetByIdsAsync(siIngredientIds))
                                    .ToDictionary(i => i.Id);

            dto.Steps = new List<InstructionStepResponse>();
            foreach (var step in steps)
            {
                var stepDto = _mapper.Map<InstructionStepResponse>(step);

                // powiązane składniki kroku
                var relatedSis = sis.Where(si => si.InstructionStepId == step.Id);
                stepDto.StepIngredients = relatedSis.Select(si => new StepIngredientResponse
                {
                    Id = si.Id,
                    InstructionStepId = si.InstructionStepId,
                    Ingredient = _mapper.Map<IngredientResponse>(allSiIngredients[si.IngredientId]),
                    Fraction = si.Fraction
                }).ToList();

                // placeholdery i ew. skalowanie tekstu
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

            // 6) Zastosuj skalowanie wartości numerycznych
            if (scale.HasValue)
            {
                var s = scale.Value;
                dto.DefaultQty *= s;
                foreach (var ri in dto.Ingredients)
                {
                    ri.Qty *= s;
                }
            }

            return dto;
        }


        public async Task<RecipeResponse> CreateAsync(NewRecipeRequest request)
        {
            // 1) jeśli jest plik, zapisz go
            if (request.ImageFile != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "recipes");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var stream = System.IO.File.Create(filePath);
                await request.ImageFile.CopyToAsync(stream);
                // ustaw w encji ścieżkę relatywną
                // zakładam, że entity ma pole Image: string
                request.Image = Path.Combine("images", "recipes", fileName).Replace("\\", "/");
            }

            // 2) mapowanie głównego przepisu
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
            // 0) Pobierz istniejącą encję i zapisz starą ścieżkę
            var existing = await _recipeRepo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Recipe {id} not found");
            var oldImagePath = existing.Image; // np. "images/recipes/abcdef.png"

            // 1) Jeśli jest nowy plik:
            if (request.ImageFile != null)
            {
                // a) usuń stary, jeżeli to nie "default.jpg"
                if (!string.IsNullOrEmpty(oldImagePath) &&
                    !oldImagePath.EndsWith("default.jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var oldFullPath = Path.Combine(_env.WebRootPath, oldImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFullPath))
                    {
                        System.IO.File.Delete(oldFullPath);
                    }
                }

                // b) zapisz nowy
                var uploads = Path.Combine(_env.WebRootPath, "images", "recipes");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var fs = System.IO.File.Create(filePath);
                await request.ImageFile.CopyToAsync(fs);

                // c) nadpisz pole Image w DTO
                request.Image = Path.Combine("images/recipes", fileName).Replace("\\", "/");
            }

            // 2) Mapowanie i zapis
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