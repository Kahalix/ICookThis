using System.Collections.Generic;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class InstructionStepResponse
    {
        public int StepOrder { get; set; }

        public int Id { get; set; }

        public int RecipeId { get; set; }

        /// <summary>
        /// Text generated after replacing the quantities in the template.
        /// </summary>
        public required string Text { get; set; }

        public List<StepIngredientResponse>? StepIngredients { get; set; }
    }
}