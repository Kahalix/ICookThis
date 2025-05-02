using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YourApp.Modules.Recipes.Entities;

namespace YourApp.Modules.Recipes.Dtos
{
    public class NewRecipeRequest
    {
        [Required, MaxLength(200)]
        public required string Name { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal DefaultQty { get; set; }

        [Required]
        public int DefaultUnitId { get; set; }

        [Required]
        public DishType DishType { get; set; }

        public required string Instructions { get; set; }

        [Required]
        public required List<RecipeIngredientRequest> Ingredients { get; set; }

        [Required]
        public required List<InstructionStepRequest> Steps { get; set; }
    }
}