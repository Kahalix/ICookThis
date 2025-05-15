using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Auth.Dtos
{
    public class ResetPasswordRequest
    {

        [Required]
        public Guid Token { get; set; }

        [Required, MinLength(8)]
        public string NewPassword { get; set; } = null!;
    }
}
