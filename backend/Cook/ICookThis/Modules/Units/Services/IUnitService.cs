using ICookThis.Modules.Units.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Units.Services
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitResponse>> GetAllAsync();
        Task<UnitResponse> GetByIdAsync(int id);
        Task<UnitResponse> CreateAsync(NewUnitRequest dto);
        Task<UnitResponse> UpdateAsync(int id, UpdateUnitRequest dto);
        Task DeleteAsync(int id);
    }
}
