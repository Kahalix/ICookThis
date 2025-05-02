using System.Collections.Generic;
using YourApp.Modules.Recipes.Entities;

namespace YourApp.Modules.Recipes.Dtos
{
    public class UpdateRecipeRequest
    {
        public required string Name { get; set; }
        public decimal? DefaultQty { get; set; }
        public int? DefaultUnitId { get; set; }
        public DishType? DishType { get; set; }
        public required string Instructions { get; set; }

        public required List<RecipeIngredientRequest> Ingredients { get; set; }
        public required List<InstructionStepRequest> Steps { get; set; }
    }
}