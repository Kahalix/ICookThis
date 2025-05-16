using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ICookThis.Modules.Users.Services;
using ICookThis.Modules.Users.Dtos;
using ICookThis.Shared.Dtos;
using ICookThis.Modules.Users.Entities;
using System.Security.Claims;

namespace ICookThis.Modules.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _svc;
        public UsersController(IUserService svc) => _svc = svc;

        [HttpGet, Authorize(Roles = "Admin,Moderator")]
        public Task<PagedResult<UserResponse>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] UserSortBy sortBy = UserSortBy.UserName,
            [FromQuery] SortOrder sortOrder = SortOrder.Asc,
            [FromQuery] UserStatus? status = null,
            [FromQuery] UserRole? role = null)
        {
            return _svc.GetPagedAsync(page, pageSize, search, sortBy, sortOrder, status, role);
        }

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult<PublicUserResponse>> GetPublic(int id)
        {
            var dto = await _svc.GetPublicByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("{id}/admin"), Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {

            var dto = await _svc.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("me"), Authorize]
        public async Task<ActionResult<UserResponse>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");

            var currentUserId = int.Parse(userIdClaim.Value);
            var dto = await _svc.GetByIdAsync(currentUserId);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> Create([FromBody] NewUserRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            var created = await _svc.CreateAsync(dto, currentUserId);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult<UserResponse>> Update(
            int id,
            [FromForm] UpdateUserRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            var updated = await _svc.UpdateAsync(id, dto, currentUserId);
            return Ok(updated);
        }

        [HttpDelete("{id}/profile-image"), Authorize]
        public async Task<IActionResult> DeleteProfileImage(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            await _svc.DeleteProfileImageAsync(id, currentUserId);
            return NoContent();
        }

        [HttpDelete("{id}/banner-image"), Authorize]
        public async Task<IActionResult> DeleteBannerImage(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            await _svc.DeleteBannerImageAsync(id, currentUserId);
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            await _svc.DeleteAsync(id, currentUserId);
            return NoContent();
        }

        [HttpPatch("{id}/status"), Authorize]
        public async Task<ActionResult<UserResponse>> ChangeStatus(
            int id,
            [FromBody] ChangeUserStatusRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            var updated = await _svc.ChangeStatusAsync(id, dto, currentUserId);
            return Ok(updated);
        }

        [HttpPatch("{id}/role"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> ChangeRole(
            int id,
            [FromBody] ChangeUseRoleRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var currentUserId = int.Parse(userIdClaim.Value);

            var updated = await _svc.ChangeRoleAsync(id, dto, currentUserId);
            return Ok(updated);
        }
    }
}
