using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ICookThis.Modules.Units.Dtos;
using ICookThis.Modules.Units.Entities;
using ICookThis.Modules.Units.Repositories;

namespace ICookThis.Modules.Units.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _repo;
        private readonly IMapper _mapper;
        public UnitService(IUnitRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UnitResponse>> GetAllAsync()
        {
            var units = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<UnitResponse>>(units);
        }

        public async Task<UnitResponse> GetByIdAsync(int id)
        {
            var unit = await _repo.GetByIdAsync(id);
            return _mapper.Map<UnitResponse>(unit);
        }

        public async Task<UnitResponse> CreateAsync(NewUnitRequest dto)
        {
            var entity = _mapper.Map<Unit>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<UnitResponse>(created);
        }

        public async Task<UnitResponse> UpdateAsync(int id, UpdateUnitRequest dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Unit {id} not found");
            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<UnitResponse>(updated);
        }

        public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
