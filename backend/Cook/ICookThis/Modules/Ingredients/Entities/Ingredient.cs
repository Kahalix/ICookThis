using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Ingredients.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }
    }
}
