using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Users.Dtos
{
    public class UpdateUserRequest
    {
        [MaxLength(100)]
        public string? UserName { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        [MinLength(8), MaxLength(100)]
        public string? Password { get; set; }

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Phone, MaxLength(100)]
        public string? PhoneNumber { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public IFormFile? ProfileImageFile { get; set; }
        
        public IFormFile? BannerImageFile { get; set; }
    }
}