using ICookThis.Modules.Ingredients.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Ingredients.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientResponse>> GetAllAsync();
        Task<IngredientResponse> GetByIdAsync(int id);
        Task<IngredientResponse> CreateAsync(NewIngredientRequest dto);
        Task<IngredientResponse> UpdateAsync(int id, UpdateIngredientRequest dto);
        Task DeleteAsync(int id);
    }
}