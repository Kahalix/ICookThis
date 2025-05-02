using System.ComponentModel.DataAnnotations.Schema;

namespace YourApp.Modules.Recipes.Entities
{
    public class RecipeIngredient
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }
        public int IngredientId { get; set; }

        [Column(TypeName = "decimal(9,3)")]
        public decimal Qty { get; set; }

        public int UnitId { get; set; }
    }
}
