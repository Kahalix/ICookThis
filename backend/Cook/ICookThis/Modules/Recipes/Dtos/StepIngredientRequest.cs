using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class StepIngredientRequest
    {
        [Required]
        public int IngredientId { get; set; }

        [Range(0.0001, 1.0)]
        public decimal Fraction { get; set; }
    }
}