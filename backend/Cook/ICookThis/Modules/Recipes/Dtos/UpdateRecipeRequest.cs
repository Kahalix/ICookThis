using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ICookThis.Modules.Recipes.Entities;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class UpdateRecipeRequest
    {
        [Required, MaxLength(200)]
        public required string Name { get; set; }

        public decimal? DefaultQty { get; set; }
        
        public int? DefaultUnitId { get; set; }
        
        public DishType? DishType { get; set; }

        [Required]
        public required string Description { get; set; }

        public string? Image { get; set; }
        
        public IFormFile? ImageFile { get; set; }
        
        public bool RemoveImage { get; set; }

        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        
        public List<UpdateInstructionStepRequest> Steps { get; set; } = new();
    }
}
