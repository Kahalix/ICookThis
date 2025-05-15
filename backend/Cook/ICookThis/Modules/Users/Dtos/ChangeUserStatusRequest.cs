using ICookThis.Modules.Users.Entities;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Users.Dtos
{
    public class ChangeUserStatusRequest
    {
        [Required]
        public UserStatus Status { get; set; }
    }
}
