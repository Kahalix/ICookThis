using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Ingredients.Services;
using ICookThis.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Ingredients.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _service;
        public IngredientsController(IIngredientService service) => _service = service;

        [HttpGet, AllowAnonymous]
        public Task<PagedResult<IngredientResponse>> GetAll(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string? search = null)
        {
            return _service.GetPagedAsync(page, pageSize, search);
        }

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult<IngredientResponse>> Get(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost, Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<IngredientResponse>> Create([FromBody] NewIngredientRequest dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IngredientResponse>> Update(int id, [FromBody] UpdateIngredientRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public Task Delete(int id) => _service.DeleteAsync(id);
    }
}