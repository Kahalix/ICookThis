using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Recipes.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/[controller]")]
    public class InstructionStepsController : ControllerBase
    {
        private readonly IInstructionStepService _service;
        public InstructionStepsController(IInstructionStepService service) => _service = service;

        [HttpGet]
        public Task<IEnumerable<InstructionStepResponse>> GetAll(int recipeId) =>
            _service.GetByRecipeAsync(recipeId);

        [HttpGet("{id}")]
        public async Task<ActionResult<InstructionStepResponse>> Get(int recipeId, int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null || dto.RecipeId != recipeId) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<InstructionStepResponse>> Create(int recipeId, [FromForm] NewInstructionStepRequest dto)
        {
            var created = await _service.CreateAsync(recipeId, dto);
            return CreatedAtAction(nameof(Get), new { recipeId, id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InstructionStepResponse>> Update(int recipeId, int id, [FromForm] UpdateInstructionStepRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public Task Delete(int recipeId, int id) => _service.DeleteAsync(id);
    }
}