namespace YourApp.Modules.Recipes.Entities
{
    public class StepIngredient
    {
        public int Id { get; set; }

        public int InstructionStepId { get; set; }
        public int IngredientId { get; set; }

        /// <summary>
        /// Ułamek całkowitej ilości składnika z przepisu (np. 0.3 = 30%)
        /// </summary>
        public decimal Fraction { get; set; }
    }
}
