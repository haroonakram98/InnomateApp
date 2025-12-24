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
        Task<IReadOnlyList<Product>> GetProductsWithCategoryAsync();
        Task<Product?> GetProductWithStockInfoAsync(int id);
        Task<IReadOnlyList<Product>> GetLowStockProductsAsync();
        Task<bool> ProductSkuExistsAsync(string sku);
        Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm);
        Task DeactivateAsync(int id);
        Task<int> CountAsync();
        Task<int> CountLowStockAsync();
    }
}
