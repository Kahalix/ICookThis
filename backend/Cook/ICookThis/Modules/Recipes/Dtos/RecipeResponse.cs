using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Units.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class RecipeResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal DefaultQty { get; set; }

        public required UnitResponse DefaultUnit { get; set; }

        public DishType DishType { get; set; }

        public required string Instructions { get; set; }

        /// <summary>
        /// Average dish rating (0–5)
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

        /// <summary>
        /// Average preparation time (in minutes)
        /// </summary>
        [Column(TypeName = "decimal(9,2)")]
        public decimal? AvgPreparationTimeMinutes { get; set; }

        public required List<RecipeIngredientResponse> Ingredients { get; set; } = new();

        public required List<InstructionStepResponse> Steps { get; set; } = new();
    }
}