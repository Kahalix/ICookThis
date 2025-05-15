using ICookThis.Modules.Recipes.Entities;
using System.ComponentModel.DataAnnotations;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class ChangeRecipeStatusRequest
    {
        [Required]
        public RecipeStatus Status { get; set; }
    }
}
