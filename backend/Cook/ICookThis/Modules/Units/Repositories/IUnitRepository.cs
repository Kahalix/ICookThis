using ICookThis.Modules.Units.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Units.Repositories
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Unit>> GetAllAsync();
        Task<Unit?> GetByIdAsync(int id);
        Task<Unit> AddAsync(Unit unit);
        Task<Unit> UpdateAsync(Unit unit);
        Task DeleteAsync(int id);
    }
}
