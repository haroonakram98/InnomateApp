namespace InnomateApp.Application.DTOs.Categories.Responses
{
    /// <summary>
    /// Response DTO representing a category
    /// </summary>
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// Lightweight category response for dropdowns/lookups
    /// </summary>
    public class CategoryLookupResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
