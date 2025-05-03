using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Ingredients.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _service;
        public IngredientsController(IIngredientService service) => _service = service;

        [HttpGet]
        public Task<IEnumerable<IngredientResponse>> GetAll() => _service.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientResponse>> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<IngredientResponse>> Create([FromBody] NewIngredientRequest dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IngredientResponse>> Update(int id, [FromBody] UpdateIngredientRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public Task Delete(int id) => _service.DeleteAsync(id);
    }
}