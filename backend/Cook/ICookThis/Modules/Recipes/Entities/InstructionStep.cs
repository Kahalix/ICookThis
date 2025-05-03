namespace ICookThis.Modules.Recipes.Entities
{
    public class InstructionStep
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        /// <summary>
        /// Step order
        /// </summary>
        public int StepOrder { get; set; }

        /// <summary>
        /// Text with placeholders, e.g. "Boil {Water}."
        /// </summary>
        public required string TemplateText { get; set; }
    }
}
