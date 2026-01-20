using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces
{
    /// <summary>
    /// Repository interface for Category operations
    /// Extends generic repository with category-specific methods
    /// </summary>
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Get category by name within a tenant
        /// </summary>
        Task<Category?> GetByNameAsync(string name, int tenantId);

        /// <summary>
        /// Get count of products in a category
        /// </summary>
        Task<int> GetProductCountAsync(int categoryId);

        /// <summary>
        /// Get all categories with product count for a tenant
        /// </summary>
        Task<List<CategoryResponse>> GetAllWithProductCountAsync(int tenantId);

        /// <summary>
        /// Get lightweight category list for dropdowns
        /// </summary>
        Task<List<CategoryLookupResponse>> GetLookupAsync(int tenantId);
    }
}

