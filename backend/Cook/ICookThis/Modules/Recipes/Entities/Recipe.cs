using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICookThis.Modules.Recipes.Entities
{
    // Unique constraint on Name and DishType (moved to CookThisDbContext due to lack of functions in EF Core)
    // [Index(nameof(Name), nameof(DishType), IsUnique = true)]
    public class Recipe
    {
        public int Id { get; set; }

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
        public string Image { get; set; } = "default.jpg";

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
    }
}