using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<Product?> GetByIdtWithStockSummaryAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.StockSummary)
                .Where(p => p.IsActive && p.ProductId == id)
                .SingleOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsWithCategoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StockSummary)
                .Where(p => p.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithStockInfoAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StockSummary)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IReadOnlyList<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.StockSummary)
                .Where(p => p.IsActive && p.StockSummary != null && p.StockSummary.Balance <= p.ReorderLevel)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ProductSkuExistsAsync(string sku)
        {
            return await _context.Products
                .AnyAsync(p => p.SKU == sku);
        }

        public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.StockSummary)
                .Where(p => p.IsActive &&
                           (p.Name.Contains(searchTerm) ||
                            p.SKU != null && p.SKU.Contains(searchTerm)))
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task DeactivateAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product is null)
                throw new KeyNotFoundException($"Product with id {id} not found.");

            product.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive);
        }

        public async Task<int> CountLowStockAsync()
        {
            return await _context.Products
                 .Where(p => p.IsActive && p.StockSummary != null && p.StockSummary.Balance <= p.ReorderLevel)
                 .CountAsync();
        }
    }
}
