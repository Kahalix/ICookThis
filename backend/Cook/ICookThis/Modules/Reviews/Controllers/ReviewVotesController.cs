using System.Security.Claims;
using System.Threading.Tasks;
using ICookThis.Modules.Reviews.Dtos;
using ICookThis.Modules.Reviews.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ICookThis.Modules.Reviews.Controllers
{
    [ApiController]
    [Route("api/reviews/{reviewId}/votes")]
    public class ReviewVotesController : ControllerBase
    {
        private readonly IReviewVoteService _svc;
        public ReviewVotesController(IReviewVoteService svc) => _svc = svc;

        [HttpPost, Authorize]
        public async Task<ActionResult<ReviewVoteDto>> Vote(
            int reviewId,
            [FromBody] NewReviewVoteRequest dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _svc.VoteAsync(reviewId, userId, dto.IsAgree);
            return Ok(result);
        }

        [HttpDelete, Authorize]
        public async Task<IActionResult> RemoveVote(int reviewId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _svc.RemoveVoteAsync(reviewId, userId);
            return NoContent();
        }
    }
}
