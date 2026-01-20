namespace InnomateApp.Application.DTOs.Categories.Requests
{
    /// <summary>
    /// Request DTO for creating a new category
    /// </summary>
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
