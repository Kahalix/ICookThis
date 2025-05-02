using System.Collections.Generic;
using YourApp.Modules.Recipes.Entities;
using YourApp.Modules.Units.Dtos;

namespace YourApp.Modules.Recipes.Dtos
{
    public class RecipeResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal DefaultQty { get; set; }

        public required UnitResponse DefaultUnit { get; set; }

        public DishType DishType { get; set; }

        public required string Instructions { get; set; }

        public decimal? AvgRating { get; set; }

        public required List<RecipeIngredientResponse> Ingredients { get; set; }

        public required List<InstructionStepResponse> Steps { get; set; }
    }
}