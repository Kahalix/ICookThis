using ICookThis.Modules.Ingredients.Dtos;
using ICookThis.Modules.Units.Dtos;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class RecipeIngredientResponse
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public required IngredientResponse Ingredient { get; set; }

        public decimal Qty { get; set; }
        public required UnitResponse Unit { get; set; }
    }
}