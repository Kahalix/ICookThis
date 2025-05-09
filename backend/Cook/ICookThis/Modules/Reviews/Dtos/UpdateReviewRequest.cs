﻿using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Reviews.Dtos
{
    public class UpdateReviewRequest
    {
        [MaxLength(100)]
        public required string Reviewer { get; set; }

        [Range(1, 5)]
        public int? Difficulty { get; set; }

        public bool? Recommend { get; set; }

        public string? Comment { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }

        [Range(1, int.MaxValue)]
        public int? PreparationTimeMinutes { get; set; }
    }
}