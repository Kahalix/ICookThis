
namespace ICookThis.Modules.Reviews.Dtos
{
    public class ModerationResponse
    {
        public bool IsSpam { get; set; }
        public bool IsToxic { get; set; }
        public decimal SpamScore { get; set; }
        public decimal ToxicityScore { get; set; }
    }
}