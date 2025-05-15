using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Auth.Dtos
{
    public class LoginRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}