using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ICookThis.Modules.Recipes.Dtos;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Recipes.Services;
using ICookThis.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Recipes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _service;
        public RecipesController(IRecipeService service) => _service = service;

        // GET /api/recipes
        [HttpGet]
        [AllowAnonymous]
        public async Task<PagedResult<RecipeListResponse>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] RecipeSortBy sortBy = RecipeSortBy.Name,
            [FromQuery] SortOrder sortOrder = SortOrder.Asc,
            [FromQuery] DishType? dishType = null,
            [FromQuery] RecipeStatus? status = null,
            [FromQuery] int? minReviewsCount = null)
        {
            int? userId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
                userId = int.Parse(userIdClaim.Value);

            return await _service.GetPagedAsync(
                page, pageSize, search,
                sortBy, sortOrder,
                dishType, status, minReviewsCount,
                userId);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RecipeResponse>> Get(
            int id,
            [FromQuery] decimal? scale = null)
        {

            int? userId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
                 userId = int.Parse(userIdClaim.Value);

            var dto = await _service.GetByIdAsync(id, userId, scale);
            return Ok(dto);
        }

        [HttpGet("user/{ownerId}")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<RecipeListResponse>>> GetByUser(
            int ownerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] RecipeSortBy sortBy = RecipeSortBy.Name,
            [FromQuery] SortOrder sortOrder = SortOrder.Asc,
            [FromQuery] DishType? dishType = null,
            [FromQuery] RecipeStatus? status = null,
            [FromQuery] int? minReviewsCount = null)
        {
            int? currentUserId = null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
                currentUserId = int.Parse(userIdClaim.Value);

            var result = await _service.GetByUserAsync(
                ownerId,
                page, pageSize,
                search,
                sortBy, sortOrder,
                dishType, status,
                minReviewsCount,
                currentUserId);

            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<PagedResult<RecipeListResponse>>> GetMyRecipes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] RecipeSortBy sortBy = RecipeSortBy.Name,
            [FromQuery] SortOrder sortOrder = SortOrder.Asc,
            [FromQuery] DishType? dishType = null,
            [FromQuery] RecipeStatus? status = null,
            [FromQuery] int? minReviewsCount = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            return await _service.GetMyRecipesAsync(
                page, pageSize, search, sortBy, sortOrder,
                dishType, status, minReviewsCount,
                currentUserId);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<RecipeResponse>> Create([FromForm] NewRecipeRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var userId = int.Parse(userIdClaim.Value);
            
            var created = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult<RecipeResponse>> Update(
            int id,
            [FromForm] UpdateRecipeRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var userId = int.Parse(userIdClaim.Value);

            var updated = await _service.UpdateAsync(id, dto, userId);
            return Ok(updated);
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<ActionResult<RecipeResponse>> ChangeStatus(
            int id,
            [FromBody] ChangeRecipeStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var userId = int.Parse(userIdClaim.Value);

            var updated = await _service.ChangeStatusAsync(
                recipeId: id,
                newStatus: request.Status,
                userId: userId);

            return Ok(updated);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public Task Delete(int id) => _service.DeleteAsync(id);
    }
}