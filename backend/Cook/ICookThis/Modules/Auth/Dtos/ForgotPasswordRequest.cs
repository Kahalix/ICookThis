using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Auth.Dtos
{
    public class ForgotPasswordRequest {

        [Required, EmailAddress]
        public string Email { get; set; } = null!; 
    }
}
