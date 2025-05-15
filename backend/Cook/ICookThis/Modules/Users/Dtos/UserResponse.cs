using ICookThis.Modules.Users.Entities;

namespace ICookThis.Modules.Users.Dtos
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
        public string? BannerImage { get; set; }
        public decimal TrustFactor { get; set; }
        public decimal ReviewTrustFactor { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
