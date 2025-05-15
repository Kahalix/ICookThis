
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
            var steps = (await _stepRepo.GetByRecipeAsync(recipeId)).OrderBy(s => s.StepOrder).ToList();
            var stepIds = steps.Select(s => s.Id).ToList();

            var allSis = (await _siRepo.GetByStepIdsAsync(stepIds)).ToList();

            var result = new List<InstructionStepResponse>(steps.Count);
            foreach (var step in steps)
            {
                var dto = _mapper.Map<InstructionStepResponse>(step);

                var sis = allSis.Where(si => si.InstructionStepId == step.Id).ToList();
                dto.StepIngredients = _mapper.Map<List<StepIngredientResponse>>(sis);

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

        public async Task<InstructionStepResponse> CreateAsync(int recipeId, NewInstructionStepRequest request)
        {
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

            var stepEntity = _mapper.Map<InstructionStep>(request);
            stepEntity.RecipeId = recipeId;
            var createdStep = await _stepRepo.AddAsync(stepEntity);

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

        public async Task<InstructionStepResponse> UpdateAsync(int id, UpdateInstructionStepRequest request)
        {
            var existing = await _stepRepo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Step {id} not found");
            var oldImagePath = existing.Image;

            var shouldRemoveOldImage = request.RemoveImage || request.ImageFile != null;

            if (shouldRemoveOldImage
                && !string.IsNullOrEmpty(oldImagePath)
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

            if (request.ImageFile != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "instructionsteps");
                Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ImageFile.FileName)}";
                var fullNewPath = Path.Combine(uploads, fileName);
                using var fs = System.IO.File.Create(fullNewPath);
                await request.ImageFile.CopyToAsync(fs);

                request.Image = Path.Combine("images", "instructionsteps", fileName)
                                 .Replace("\\", "/");
            }
            else if (request.RemoveImage)
            {
                request.Image = null;
            }

            _mapper.Map(request, existing);
            await _stepRepo.UpdateAsync(existing);

            var oldSis = await _siRepo.GetByStepAsync(id);
            foreach (var osi in oldSis)
                await _siRepo.DeleteAsync(osi.Id);

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

        public async Task DeleteAsync(int id)
        {
            var step = await _stepRepo.GetByIdAsync(id);
            if (step == null) return;

            if (!string.IsNullOrEmpty(step.Image)
                && !step.Image.EndsWith("default.jpg", StringComparison.OrdinalIgnoreCase))
            {
                var fullPath = Path.Combine(
                    _env.WebRootPath,
                    step.Image.Replace("/", Path.DirectorySeparatorChar.ToString())
                );
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }

            await _stepRepo.DeleteAsync(id);
        }
    }
}