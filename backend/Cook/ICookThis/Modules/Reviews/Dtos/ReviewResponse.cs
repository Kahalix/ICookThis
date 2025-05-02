using System;

namespace YourApp.Modules.Reviews.Dtos
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public required string Reviewer { get; set; }
        public int Difficulty { get; set; }
        public bool Recommend { get; set; }
        public string? Comment { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
