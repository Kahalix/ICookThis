using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ICookThis.Modules.Recipes.Entities;
using ICookThis.Modules.Units.Dtos;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class RecipeResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;
        public decimal TrustFactor { get; set; }
        public AddedBy AddedBy { get; set; }
        public RecipeStatus Status { get; set; }
        public required string Name { get; set; }
        public decimal DefaultQty { get; set; }
        public required UnitResponse DefaultUnit { get; set; }
        public DishType DishType { get; set; }
        public required string Description { get; set; }

        public string? Image { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? AvgRating { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? AvgDifficulty { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? RecommendPercentage { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal? AvgPreparationTimeMinutes { get; set; }

        public int? ReviewsCount { get; set; }

        public required List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public required List<InstructionStepResponse> Steps { get; set; } = new();
    }
}