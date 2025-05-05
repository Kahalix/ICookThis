using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Ingredients.Dtos
{
    public class UpdateIngredientRequest
    {
        [MaxLength(100)]
        public required string Name { get; set; }
    }
}