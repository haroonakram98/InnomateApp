using InnomateApp.Application.DTOs.Products.Responses;
using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetByIdtWithStockSummaryAsync(int id);
        /// <summary>
        /// Gets active products with category and stock info (for POS)
        /// </summary>
        Task<IReadOnlyList<Product>> GetActiveProductsWithCategoryAsync();
        
        /// <summary>
        /// Gets ALL products with category and stock info (for Admin)
        /// </summary>
        Task<IReadOnlyList<Product>> GetAllProductsWithCategoryAsync();

        /// <summary>
        /// Gets lightweight product list for dropdowns (ID, Name, SKU, Price)
        /// </summary>
        Task<IReadOnlyList<ProductLookupResponse>> GetLookupAsync();

        Task<Product?> GetProductWithStockInfoAsync(int id);
        Task<IReadOnlyList<Product>> GetLowStockProductsAsync();
        Task<bool> ProductSkuExistsAsync(string sku);
        Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm);
        Task DeactivateAsync(int id);
        Task<int> CountAsync();
        Task<int> CountLowStockAsync();
    }
}
