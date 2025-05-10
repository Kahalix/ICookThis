using ICookThis.Modules.Recipes.Entities;
using System.Collections.Generic;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class UpdateRecipeRequest
    {
        public required string Name { get; set; }
        public decimal? DefaultQty { get; set; }
        public int? DefaultUnitId { get; set; }
        public DishType? DishType { get; set; }
        public required string Description { get; set; }
        public string Image{ get; set; } = "default.jpg";
        public IFormFile? ImageFile { get; set; }
        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<UpdateInstructionStepRequest> Steps { get; set; } = new();
        public bool RemoveImage { get; set; }
    }
}