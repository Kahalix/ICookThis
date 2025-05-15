using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICookThis.Modules.Recipes.Entities
{
    // [Index(nameof(Name), nameof(DishType), IsUnique = true)]  Moved to DBContext
    public class Recipe
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public AddedBy AddedBy { get; set; } = AddedBy.User;

        public RecipeStatus Status = RecipeStatus.Pending;

        [Required, MaxLength(200)]
        public required string Name { get; set; }

        [Column(TypeName = "decimal(9,3)")]
        public decimal DefaultQty { get; set; }

        public int DefaultUnitId { get; set; }

        [Required]
        public DishType DishType { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public required string Description { get; set; }

        [MaxLength(200)]
        public string? Image { get; set; }

        /// <summary>
        /// Average dish rating (1–5)
        /// </summary>
        [Column(TypeName = "decimal(3,2)")]
        public decimal? AvgRating { get; set; }

        /// <summary>
        /// Average difficulty (1–5)
        /// </summary>
        [Column(TypeName = "decimal(3,2)")]
        public decimal? AvgDifficulty { get; set; }

        /// <summary>
        /// Percentage of positive reviews (0–100)
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal? RecommendPercentage { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal? AvgPreparationTimeMinutes { get; set; }

        public int? ReviewsCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}