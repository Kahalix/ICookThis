namespace ICookThis.Modules.Recipes.Entities
{
    public class StepIngredient
    {
        public int Id { get; set; }

        public int InstructionStepId { get; set; }
        public int IngredientId { get; set; }

        /// <summary>
        /// Fraction of the total amount of the ingredient in the recipe (e.g. 0.3 = 30%)
        /// </summary>
        public decimal Fraction { get; set; }
    }
}