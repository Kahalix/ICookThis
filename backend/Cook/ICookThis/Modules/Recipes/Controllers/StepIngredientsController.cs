using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Recipes.Controllers
{
    [ApiController]
    [Route("api/steps/{stepId}/[controller]")]
    public class StepIngredientsController : ControllerBase
    {
        private readonly IStepIngredientService _service;
        public StepIngredientsController(IStepIngredientService service) => _service = service;

        [HttpGet, Authorize(Roles = "Admin,Moderator")]
        public Task<IEnumerable<StepIngredientResponse>> GetAll(int stepId) =>
            _service.GetByStepAsync(stepId);

        [HttpGet("{id}"), Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<StepIngredientResponse>> Get(int stepId, int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null || dto.InstructionStepId != stepId) return NotFound();
            return Ok(dto);
        }

        [HttpPost, Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<StepIngredientResponse>> Create(int stepId, [FromBody] StepIngredientRequest dto)
        {
            var created = await _service.CreateAsync(stepId, dto);
            return CreatedAtAction(nameof(Get), new { stepId, id = created.Id }, created);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<StepIngredientResponse>> Update(int stepId, int id, [FromBody] StepIngredientRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,Moderator")]
        public Task Delete(int stepId, int id) => _service.DeleteAsync(id);
    }
}
