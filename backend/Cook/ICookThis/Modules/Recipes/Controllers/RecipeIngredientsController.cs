using System.Collections.Generic;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Recipes.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/[controller]")]
    public class RecipeIngredientsController : ControllerBase
    {
        private readonly IRecipeIngredientService _service;
        public RecipeIngredientsController(IRecipeIngredientService service)
            => _service = service;

        [HttpGet, Authorize(Roles = "Admin,Moderator")]
        public Task<IEnumerable<RecipeIngredientResponse>> GetAll(int recipeId) =>
            _service.GetByRecipeAsync(recipeId);

        [HttpGet("{id}"), Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<RecipeIngredientResponse>> Get(int recipeId, int id)
        {
            var ri = await _service.GetByIdAsync(id);
            if (ri == null || ri.RecipeId != recipeId) return NotFound();
            return Ok(ri);
        }

        [HttpPost, Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<RecipeIngredientResponse>> Create(
            int recipeId,
            [FromBody] RecipeIngredientRequest dto)
        {
            var created = await _service.CreateAsync(recipeId, dto);
            return CreatedAtAction(nameof(Get), new { recipeId, id = created.Id }, created);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<RecipeIngredientResponse>> Update(
            int recipeId,
            int id,
            [FromBody] RecipeIngredientRequest dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin,Moderator")]
        public Task Delete(int recipeId, int id) => _service.DeleteAsync(id);
    }
}