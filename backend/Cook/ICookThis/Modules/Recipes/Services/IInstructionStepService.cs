using ICookThis.Modules.Recipes.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICookThis.Modules.Recipes.Services
{
    public interface IInstructionStepService
    {
        Task<IEnumerable<InstructionStepResponse>> GetByRecipeAsync(int recipeId);
        Task<InstructionStepResponse> GetByIdAsync(int id);
        Task<InstructionStepResponse> CreateAsync(int recipeId, InstructionStepRequest dto);
        Task<InstructionStepResponse> UpdateAsync(int id, InstructionStepRequest dto);
        Task DeleteAsync(int id);
    }
}