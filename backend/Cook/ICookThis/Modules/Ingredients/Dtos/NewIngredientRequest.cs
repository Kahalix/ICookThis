using System.ComponentModel.DataAnnotations;

namespace YourApp.Modules.Ingredients.Dtos
{
    public class NewIngredientRequest
    {
        [Required, MaxLength(100)]
        public required string Name { get; set; }
    }
}
