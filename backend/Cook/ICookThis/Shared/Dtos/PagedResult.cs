using ICookThis.Modules.Recipes.Dtos;

namespace ICookThis.Shared.Dtos
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public RecipeSortBy? SortBy { get; set; }
        public SortOrder? SortOrder { get; set; }
        public string? Search { get; set; }
        public IEnumerable<T> Items { get; set; } = [];
        
    }
}