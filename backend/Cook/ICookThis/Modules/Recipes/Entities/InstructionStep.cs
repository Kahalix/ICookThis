using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Recipes.Entities
{
    public class InstructionStep
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        [MaxLength(200)]
        public string? Image { get; set; }

        public int StepOrder { get; set; }

        /// <summary>
        /// Text with placeholders, e.g. "Boil {Water}."
        /// </summary>
        public required string TemplateText { get; set; }
    }
}
