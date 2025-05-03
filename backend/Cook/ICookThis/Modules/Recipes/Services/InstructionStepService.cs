
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Repositories;

namespace ICookThis.Modules.Recipes.Services
{
    public class InstructionStepService : IInstructionStepService
    {
        private readonly IInstructionStepRepository _stepRepo;
        private readonly IStepIngredientRepository _siRepo;
        private readonly IMapper _mapper;

        public InstructionStepService(
            IInstructionStepRepository stepRepo,
            IStepIngredientRepository siRepo,
            IMapper mapper)
        {
            _stepRepo = stepRepo;
            _siRepo = siRepo;
            _mapper = mapper;
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
            dto.StepIngredients = _mapper.Map<List<StepIngredientResponse>>(sis);
            dto.Text = step.TemplateText;

            return dto;
        }

        public async Task<InstructionStepResponse> CreateAsync(int recipeId, InstructionStepRequest request)
        {
            // mapowanie podstawowych pól kroku
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
            var existing = await _stepRepo.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Step {id} not found");

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
