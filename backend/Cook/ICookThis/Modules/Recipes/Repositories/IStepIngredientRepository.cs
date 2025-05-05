using ICookThis.Modules.Recipes.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Repositories
{
    public interface IStepIngredientRepository
    {
        Task<IEnumerable<StepIngredient>> GetByStepAsync(int stepId);
        Task<StepIngredient?> GetByIdAsync(int id);
        Task<StepIngredient> AddAsync(StepIngredient si);
        Task<StepIngredient> UpdateAsync(StepIngredient si);
        Task DeleteAsync(int id);
        Task<IEnumerable<StepIngredient>> GetByStepIdsAsync(IEnumerable<int> stepIds);
    }
}