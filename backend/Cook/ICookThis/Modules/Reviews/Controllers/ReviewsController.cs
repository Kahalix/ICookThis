using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Services;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Reviews.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;
        public ReviewsController(IReviewService service) => _service = service;

        [HttpGet]
        public Task<IEnumerable<ReviewResponse>> GetAll(int recipeId) =>
            _service.GetByRecipeAsync(recipeId);

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponse>> Get(int recipeId, int id)
        {
            var r = await _service.GetByIdAsync(id);
            if (r == null || r.RecipeId != recipeId) return NotFound();
            return Ok(r);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponse>> Create(int recipeId, [FromBody] NewReviewRequest dto)
        {
            dto.RecipeId = recipeId;
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { recipeId, id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewResponse>> Update(int recipeId, int id, [FromBody] UpdateReviewRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public Task Delete(int recipeId, int id) => _service.DeleteAsync(id);
    }
}
