﻿using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Reviews.Dtos
{
    public class NewReviewRequest
    {
        [Required]
        public int RecipeId { get; set; }

        [Required, MaxLength(100)]
        public string Reviewer { get; set; } = null!;

        [Range(1, 5)]
        public int Difficulty { get; set; }

        public bool Recommend { get; set; }

        public string? Comment { get; set; }

        [Range(1, 5)]
        public decimal Rating { get; set; }

        [Range(1, int.MaxValue)]
        public int PreparationTimeMinutes { get; set; }
    }
}
