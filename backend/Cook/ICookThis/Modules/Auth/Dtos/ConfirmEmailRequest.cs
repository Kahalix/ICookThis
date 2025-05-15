using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Auth.Dtos
{
    public class ConfirmEmailRequest {

        [Required]
        public Guid Token { get; set; } 
    }
}
