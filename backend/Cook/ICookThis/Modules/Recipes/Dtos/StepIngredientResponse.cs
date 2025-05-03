using ICookThis.Modules.Ingredients.Dtos;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class StepIngredientResponse
    {
        public int Id { get; set; }
        public int InstructionStepId { get; set; }
        public required IngredientResponse Ingredient { get; set; }

        public decimal Fraction { get; set; }
    }
}