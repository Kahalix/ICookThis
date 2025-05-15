using ICookThis.Modules.Recipes.Entities;

namespace ICookThis.Modules.Recipes.Dtos
{
    public class RecipeListResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DishType DishType { get; set; }
        public RecipeStatus Status { get; set; }
        public int? ReviewsCount { get; set; }
        public decimal? AvgRating { get; set; }
        public string UserName { get; set; } = null!;
        public decimal TrustFactor { get; set; }
    }
}