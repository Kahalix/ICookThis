namespace ICookThis.Modules.Users.Dtos
{
    public class PublicUserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? Description { get; set; }
        public string? BannerImage { get; set; }
        public decimal TrustFactor { get; set; }
        public decimal ReviewTrustFactor { get; set; }
    }
}
