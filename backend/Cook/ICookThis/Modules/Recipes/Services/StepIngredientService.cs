using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Repositories;

namespace ICookThis.Modules.Recipes.Services
{
    public class StepIngredientService : IStepIngredientService
    {
        private readonly IStepIngredientRepository _repo;
        private readonly IMapper _mapper;
        public StepIngredientService(IStepIngredientRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StepIngredientResponse>> GetByStepAsync(int stepId)
        {
            var list = await _repo.GetByStepAsync(stepId);
            return _mapper.Map<IEnumerable<StepIngredientResponse>>(list);
        }

        public async Task<StepIngredientResponse> GetByIdAsync(int id)
        {
            var si = await _repo.GetByIdAsync(id);
            return _mapper.Map<StepIngredientResponse>(si);
        }

        public async Task<StepIngredientResponse> CreateAsync(int stepId, StepIngredientRequest dto)
        {
            var entity = _mapper.Map<StepIngredient>(dto);
            entity.InstructionStepId = stepId;
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<StepIngredientResponse>(created);
        }

        public async Task<StepIngredientResponse> UpdateAsync(int id, StepIngredientRequest dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            _mapper.Map(dto, existing);
            if (existing == null) throw new KeyNotFoundException($"StepIngredient with id {id} not found.");
            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<StepIngredientResponse>(updated);
        }

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
