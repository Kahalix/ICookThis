namespace ICookThis.Modules.Reviews.Entities
{
    public class ReviewVote
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public bool IsAgree { get; set; }
    }
}
