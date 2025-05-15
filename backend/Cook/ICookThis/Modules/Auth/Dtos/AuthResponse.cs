namespace ICookThis.Modules.Auth.Dtos
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}