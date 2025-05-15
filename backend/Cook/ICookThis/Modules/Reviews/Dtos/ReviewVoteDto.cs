namespace ICookThis.Modules.Reviews.Dtos
{
    public class ReviewVoteDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public bool IsAgree { get; set; }
    }

}
