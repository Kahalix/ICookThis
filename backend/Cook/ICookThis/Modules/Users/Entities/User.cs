using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Users.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; } = null!;
        
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = null!;
        
        [Required, MaxLength(100)]
        public string Password { get; set; } = null!;
        
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;
        
        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;
        
        [MaxLength(100)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [MaxLength(200)]
        public string? ProfileImage { get; set; }
        
        [MaxLength(200)]
        public string? BannerImage { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal TrustFactor { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        
        public decimal ReviewTrustFactor { get; set; }
        
        public UserStatus Status { get; set; } = UserStatus.Pending;
        
        public UserRole Role { get; set; } = UserRole.User;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
