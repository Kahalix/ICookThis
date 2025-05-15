using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Auth.Dtos
{
    public class RegisterRequest
    {
        [Required, MaxLength(100)]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Phone, MaxLength(100)]
        public string? PhoneNumber { get; set; }
    }
}