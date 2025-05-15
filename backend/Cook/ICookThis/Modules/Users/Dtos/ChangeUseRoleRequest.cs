using ICookThis.Modules.Users.Entities;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Users.Dtos
{
    public class ChangeUseRoleRequest
    {
        [Required]
        public UserRole Role { get; set; }
    }
}
