using System;
using System.ComponentModel.DataAnnotations;

namespace YourApp.Modules.Reviews.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        [Required, MaxLength(100)]
        public required string Reviewer { get; set; }

        [Range(1, 5)]
        public int Difficulty { get; set; }

        public bool Recommend { get; set; }

        public string? Comment { get; set; }

        [Range(1, 5)]
        public decimal Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
