using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourApp.Modules.Recipes.Dtos
{
    public class InstructionStepRequest
    {
        [Range(1, int.MaxValue)]
        public int StepOrder { get; set; }

        [Required, MaxLength(1000)]
        public required string TemplateText { get; set; }

        [Required]
        public List<StepIngredientRequest>? StepIngredients { get; set; }
    }
}