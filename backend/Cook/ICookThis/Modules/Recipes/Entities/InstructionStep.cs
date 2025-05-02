namespace YourApp.Modules.Recipes.Entities
{
    public class InstructionStep
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        /// <summary>
        /// Kolejność kroku
        /// </summary>
        public int StepOrder { get; set; }

        /// <summary>
        /// Tekst z placeholderami, np. "Zagotuj {Water}."
        /// </summary>
        public required string TemplateText { get; set; }
    }
}
