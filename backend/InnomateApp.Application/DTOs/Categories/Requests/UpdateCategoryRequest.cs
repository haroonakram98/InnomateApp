namespace InnomateApp.Application.DTOs.Categories.Requests
{
    /// <summary>
    /// Request DTO for updating an existing category
    /// </summary>
    public class UpdateCategoryRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
