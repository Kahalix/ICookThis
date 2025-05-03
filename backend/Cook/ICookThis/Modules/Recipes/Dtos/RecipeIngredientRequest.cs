using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class RecipeIngredientRequest
    {
        [Required]
        public int IngredientId { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal Qty { get; set; }

        [Required]
        public int UnitId { get; set; }
    }
}
