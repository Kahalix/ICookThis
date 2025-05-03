using ICookThis.Modules.Recipes.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Services
{
    public interface IStepIngredientService
    {
        Task<IEnumerable<StepIngredientResponse>> GetByStepAsync(int stepId);
        Task<StepIngredientResponse> GetByIdAsync(int id);
        Task<StepIngredientResponse> CreateAsync(int stepId, StepIngredientRequest dto);
        Task<StepIngredientResponse> UpdateAsync(int id, StepIngredientRequest dto);
        Task DeleteAsync(int id);
    }
}