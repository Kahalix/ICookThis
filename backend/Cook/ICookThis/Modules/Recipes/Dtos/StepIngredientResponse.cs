using YourApp.Modules.Ingredients.Dtos;

namespace YourApp.Modules.Recipes.Dtos
{
    public class StepIngredientResponse
    {
        public int IngredientId { get; set; }
        public required IngredientResponse Ingredient { get; set; }

        public decimal Fraction { get; set; }
    }
}