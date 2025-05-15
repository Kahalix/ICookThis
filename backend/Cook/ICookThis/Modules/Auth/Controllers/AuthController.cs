using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ICookThis.Modules.Auth.Dtos;
using ICookThis.Modules.Auth.Services;

namespace ICookThis.Modules.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _svc;
        public AuthController(IAuthService svc) => _svc = svc;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            await _svc.RegisterAsync(dto);
            return Accepted();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest dto)
        {
            if (await _svc.ConfirmEmailAsync(dto.Token))
                return Ok();
            return BadRequest("Invalid or expired token");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest dto)
        {
            var auth = await _svc.LoginAsync(dto);
            return Ok(auth);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            await _svc.ForgotPasswordAsync(dto);
            return Accepted();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
        {
            if (await _svc.ResetPasswordAsync(dto))
                return Ok();
            return BadRequest("Invalid or expired token");
        }
    }
}
