using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourApp.Modules.Recipes.Entities
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public required string Name { get; set; }

        [Column(TypeName = "decimal(9,3)")]
        public decimal DefaultQty { get; set; }

        public int DefaultUnitId { get; set; }

        [Required]
        public DishType DishType { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public required string Instructions { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? AvgRating { get; set; }
    }
}
