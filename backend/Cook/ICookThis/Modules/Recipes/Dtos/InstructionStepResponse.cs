using System.Collections.Generic;

namespace YourApp.Modules.Recipes.Dtos
{
    public class InstructionStepResponse
    {
        public int StepOrder { get; set; }

        /// <summary>
        /// Tekst wygenerowany po podstawieniu przeliczonych ilości do szablonu.
        /// </summary>
        public required string Text { get; set; }

        public List<StepIngredientResponse>? StepIngredients { get; set; }
    }
}