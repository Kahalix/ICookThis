using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Recipes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _service;
        public RecipesController(IRecipeService service) => _service = service;

        [HttpGet]
        public Task<IEnumerable<RecipeResponse>> GetAll() => _service.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeResponse>> Get(int id, [FromQuery] decimal? scale = null)
        {
            var dto = await _service.GetByIdAsync(id, scale);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<RecipeResponse>> Create([FromBody] NewRecipeRequest dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RecipeResponse>> Update(int id, [FromBody] UpdateRecipeRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public Task Delete(int id) => _service.DeleteAsync(id);
    }
}
