using ICookThis.Modules.Recipes.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Recipes.Dtos
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

        public required string Description { get; set; }
        
        public string Image { get; set; } = "default.jpg";

        public IFormFile? ImageFile { get; set; }

        public List<RecipeIngredientRequest> Ingredients { get; set; } = new List<RecipeIngredientRequest>();

        public List<InstructionStepRequest> Steps { get; set; } = new List<InstructionStepRequest>();
    }
}