using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Users.Dtos
{
    public class NewUserRequest
    {
        [Required, MaxLength(100)]
        public string UserName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Phone, MaxLength(100)]
        public string? PhoneNumber { get; set; }
    }
}
