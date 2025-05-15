using System.Security.Claims;
using System.Threading.Tasks;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Entities;
using ICookThis.Modules.Reviews.Services;
using ICookThis.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Reviews.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _svc;
        public ReviewsController(IReviewService svc) => _svc = svc;

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<PagedResult<ReviewResponse>>> GetAll(
            int recipeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] ReviewSortBy sortBy = ReviewSortBy.CreatedAt,
            [FromQuery] SortOrder sortOrder = SortOrder.Desc,
            [FromQuery] ReviewStatus? status = null)
        {
            int? uid = User.FindFirst(ClaimTypes.NameIdentifier) is { } c
                       ? int.Parse(c.Value)
                       : (int?)null;

            var result = await _svc.GetPagedByRecipeAsync(
                recipeId, page, pageSize,
                search, sortBy, sortOrder,
                status, uid);

            return Ok(result);
        }

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult<ReviewResponse>> Get(int recipeId, int id)
        {
            int? uid = User.FindFirst(ClaimTypes.NameIdentifier) is { } c
                       ? int.Parse(c.Value)
                       : (int?)null;

            var r = await _svc.GetByIdAsync(id, uid);
            if (r.RecipeId != recipeId) return NotFound();
            return Ok(r);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<ReviewResponse>> Create(
            int recipeId,
            [FromBody] NewReviewRequest dto)
        {
            var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            dto.RecipeId = recipeId;
            var created = await _svc.CreateAsync(dto, uid);
            return CreatedAtAction(nameof(Get), new { recipeId, id = created.Id }, created);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult<ReviewResponse>> Update(
            int recipeId,
            int id,
            [FromBody] UpdateReviewRequest dto)
        {
            var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var updated = await _svc.UpdateAsync(id, dto, uid);
            if (updated.RecipeId != recipeId) return BadRequest();
            return Ok(updated);
        }

        [HttpGet("my"), Authorize]
        public async Task<ActionResult<PagedResult<ReviewResponse>>> GetMyReviews(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] ReviewSortBy sortBy = ReviewSortBy.CreatedAt,
            [FromQuery] SortOrder sortOrder = SortOrder.Desc,
            [FromQuery] ReviewStatus? status = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _svc.GetMyReviewsAsync(
                page, pageSize, search, sortBy, sortOrder, status, userId);
            return Ok(result);
        }

        [HttpPatch("{id}/status"), Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ReviewResponse>> ChangeStatus(
            int id,
            [FromBody] ChangeReviewStatusRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var updated = await _svc.ChangeStatusAsync(id, dto.Status, userId);
            return Ok(updated);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> Delete(int recipeId, int id)
        {
            var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _svc.DeleteAsync(id, uid);
            return NoContent();
        }
    }
}