
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Repositories;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Repositories;

namespace ICookThis.Modules.Recipes.Services
{
    public class InstructionStepService : IInstructionStepService
    {
        private readonly IInstructionStepRepository _stepRepo;
        private readonly IStepIngredientRepository _siRepo;
        private readonly IIngredientRepository _ingredientRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public InstructionStepService(
            IInstructionStepRepository stepRepo,
            IStepIngredientRepository siRepo,
            IIngredientRepository ingredientRepo,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _stepRepo = stepRepo;
            _siRepo = siRepo;
            _ingredientRepo = ingredientRepo;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IEnumerable<InstructionStepResponse>> GetByRecipeAsync(int recipeId)
        {
            var steps = await _stepRepo.GetByRecipeAsync(recipeId);
            var result = new List<InstructionStepResponse>(steps.Count());

            foreach (var step in steps.OrderBy(s => s.StepOrder))
            {
                var dto = _mapper.Map<InstructionStepResponse>(step);

                var sis = await _siRepo.GetByStepAsync(step.Id);
                dto.StepIngredients = _mapper.Map<List<StepIngredientResponse>>(sis);

                // domyślnie tekst = szablon
                dto.Text = step.TemplateText;

                result.Add(dto);
            }

            return result;
        }

        public async Task<InstructionStepResponse> GetByIdAsync(int id)
        {
            var step = await _stepRepo.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException($"Step {id} not found");

            var dto = _mapper.Map<InstructionStepResponse>(step);

            var sis = await _siRepo.GetByStepAsync(step.Id);
            var ingredientIds = sis.Select(si => si.IngredientId).Distinct();

            var ingredientsDict = (await _ingredientRepo.GetByIdsAsync(ingredientIds))
                                  .ToDictionary(i => i.Id);

            dto.StepIngredients = sis.Select(si => new StepIngredientResponse
            {
                Id = si.Id,
                InstructionStepId = si.InstructionStepId,
                Ingredient = _mapper.Map<IngredientResponse>(ingredientsDict[si.IngredientId]),
                Fraction = si.Fraction
            }).ToList();

            dto.Text = step.TemplateText;
            return dto;
        }


        public async Task<InstructionStepResponse> CreateAsync(int recipeId, InstructionStepRequest request)
        {
            // 1) obsługa uploadu
            if (request.ImageFile != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "instructionsteps");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var stream = System.IO.File.Create(filePath);
                await request.ImageFile.CopyToAsync(stream);
                request.Image = Path.Combine("images", "instructionsteps", fileName).Replace("\\", "/");
            }

            // 2) mapowanie
            var stepEntity = _mapper.Map<InstructionStep>(request);
            stepEntity.RecipeId = recipeId;
            var createdStep = await _stepRepo.AddAsync(stepEntity);

            // mapowanie i zapis składników kroku
            var siEntities = (request.StepIngredients
                          ?? Enumerable.Empty<StepIngredientRequest>())
                .Select(siDto =>
                {
                    var e = _mapper.Map<StepIngredient>(siDto);
                    e.InstructionStepId = createdStep.Id;
                    return e;
                });

            foreach (var si in siEntities)
                await _siRepo.AddAsync(si);

            return await GetByIdAsync(createdStep.Id);
        }

        public async Task<InstructionStepResponse> UpdateAsync(int id, InstructionStepRequest request)
        {
            // 1) Pobierz istniejącą encję i zapamiętaj starą ścieżkę obrazka
            var existing = await _stepRepo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Step {id} not found");
            var oldImagePath = existing.Image;

            // 2) Obsługa nowego pliku
            if (request.ImageFile != null)
            {
                // 2a) Usuń stary obraz (jeśli nie "default.jpg")
                if (!string.IsNullOrEmpty(oldImagePath)
                    && !oldImagePath.EndsWith("default.jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var fullOldPath = Path.Combine(
                        _env.WebRootPath,
                        oldImagePath.Replace("/", Path.DirectorySeparatorChar.ToString())
                    );
                    if (System.IO.File.Exists(fullOldPath))
                    {
                        System.IO.File.Delete(fullOldPath);
                    }
                }

                // 2b) Zapisz nowy plik
                var uploads = Path.Combine(_env.WebRootPath, "images", "instructionsteps");
                Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var fullNewPath = Path.Combine(uploads, fileName);
                using var fs = System.IO.File.Create(fullNewPath);
                await request.ImageFile.CopyToAsync(fs);

                // 2c) Nadpisz ścieżkę w DTO
                request.Image = Path.Combine("images", "instructionsteps", fileName)
                                 .Replace("\\", "/");
            }

            _mapper.Map(request, existing);
            await _stepRepo.UpdateAsync(existing);

            // usuń stare składniki
            var oldSis = await _siRepo.GetByStepAsync(id);
            foreach (var osi in oldSis)
                await _siRepo.DeleteAsync(osi.Id);

            // dodaj nowe
            var newSis = (request.StepIngredients
                          ?? Enumerable.Empty<StepIngredientRequest>())
                .Select(siDto =>
                {
                    var e = _mapper.Map<StepIngredient>(siDto);
                    e.InstructionStepId = id;
                    return e;
                });

            foreach (var si in newSis)
                await _siRepo.AddAsync(si);

            return await GetByIdAsync(id);
        }

        public Task DeleteAsync(int id)
        {
            // kaskadowe usunięcie składników kroku zdefiniowane w DbContext
            return _stepRepo.DeleteAsync(id);
        }
    }
}
