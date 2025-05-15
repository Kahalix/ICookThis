using System;
using ICookThis.Modules.Reviews.Entities;

namespace ICookThis.Modules.Reviews.Dtos
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string Reviewer { get; set; } = null!;
        public int Difficulty { get; set; }
        public bool Recommend { get; set; }
        public string? Comment { get; set; }
        public decimal Rating { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public ReviewStatus Status { get; set; }
        public int AgreeCount { get; set; }
        public int DisagreeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Helpfulness
            => (AgreeCount + DisagreeCount) > 0
               ? Math.Round((decimal)AgreeCount / (AgreeCount + DisagreeCount), 2)
               : 0m;

        public int Popularity
            => AgreeCount + DisagreeCount;
    }
}
