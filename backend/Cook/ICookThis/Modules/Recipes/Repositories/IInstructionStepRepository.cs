using ICookThis.Modules.Recipes.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Repositories
{
    public interface IInstructionStepRepository
    {
        Task<IEnumerable<InstructionStep>> GetByRecipeAsync(int recipeId);
        Task<InstructionStep?> GetByIdAsync(int id);
        Task<InstructionStep> AddAsync(InstructionStep step);
        Task<InstructionStep> UpdateAsync(InstructionStep step);
        Task DeleteAsync(int id);
        Task<IEnumerable<InstructionStep>> GetByRecipeIdsAsync(IEnumerable<int> recipeIds);

    }
}
