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
        public required string Instructions { get; set; }

        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
        public List<InstructionStepRequest> Steps { get; set; } = new();
    }
}