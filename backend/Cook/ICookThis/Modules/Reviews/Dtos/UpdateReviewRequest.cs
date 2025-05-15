using System.ComponentModel.DataAnnotations;
using ICookThis.Modules.Reviews.Entities;

namespace ICookThis.Modules.Reviews.Dtos
{
    public class UpdateReviewRequest
    {
        [MaxLength(100)]
        public string? Reviewer { get; set; }

        [Range(1, 5)]
        public int? Difficulty { get; set; }

        public bool? Recommend { get; set; }

        public string? Comment { get; set; }

        [Range(1, 5)]
        public decimal? Rating { get; set; }

        [Range(1, int.MaxValue)]
        public int? PreparationTimeMinutes { get; set; }

        public ReviewStatus? Status { get; set; }
    }
}