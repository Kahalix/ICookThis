using YourApp.Modules.Ingredients.Dtos;
using YourApp.Modules.Units.Dtos;

namespace YourApp.Modules.Recipes.Dtos
{
    public class RecipeIngredientResponse
    {
        public int IngredientId { get; set; }
        public required IngredientResponse Ingredient { get; set; }

        public decimal Qty { get; set; }

        public int UnitId { get; set; }
        public required UnitResponse Unit { get; set; }
    }
}