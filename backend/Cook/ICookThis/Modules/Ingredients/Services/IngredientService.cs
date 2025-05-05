using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Entities;
using ICookThis.Modules.Ingredients.Repositories;
using ICookThis.Shared.Dtos;

namespace ICookThis.Modules.Ingredients.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repo;
        private readonly IMapper _mapper;
        public IngredientService(IIngredientRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<PagedResult<IngredientResponse>> GetPagedAsync(
            int page,
            int pageSize,
            string? search)
        {
            var (entities, total) = await _repo.GetPagedAsync(page, pageSize, search);

            var dtos = entities
                .Select(e => _mapper.Map<IngredientResponse>(e))
                .ToList();

            return new PagedResult<IngredientResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)System.Math.Ceiling(total / (double)pageSize),
                Search = search,
                Items = dtos
            };
        }

        public async Task<IEnumerable<IngredientResponse>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<IngredientResponse>>(items);
        }

        public async Task<IngredientResponse> GetByIdAsync(int id)
        {
            var ing = await _repo.GetByIdAsync(id);
            return _mapper.Map<IngredientResponse>(ing);
        }

        public async Task<IngredientResponse> CreateAsync(NewIngredientRequest dto)
        {
            var entity = _mapper.Map<Ingredient>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<IngredientResponse>(created);
        }

        public async Task<IngredientResponse> UpdateAsync(int id, UpdateIngredientRequest dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Ingredient with id {id} not found.");
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<IngredientResponse>(updated);
        }

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}