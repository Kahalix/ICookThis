using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ICookThis.Modules.Recipes.Entities;

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

        [Required]
        public required string Description { get; set; }

        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }

        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<NewInstructionStepRequest> Steps { get; set; } = new();
    }
}
