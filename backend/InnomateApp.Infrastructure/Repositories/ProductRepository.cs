using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.Include(p => p.Category).ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products.Include(p => p.Category)
                                   .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<Product?> GetByIdWithCategoryAsync(int id) =>
            await _context.Products.Include(p => p.Category)
                                   .Include(p => p.StockSummary)
                                   .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<IEnumerable<Product>> GetAllWithStockAsync() =>
            await _context.Products.Include(p => p.Category)
                                   .Include(p => p.StockSummary)
                                   .ToListAsync();

        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
        }

        public void Update(Product entity)
        {
            _context.Products.Update(entity);
        }

        public void Delete(Product entity)
        {
            _context.Products.Remove(entity);
        }

        public async Task<Product?> GetByIdWithStockAsync(int id)
        {
            return await _context.Products
                .Include(p => p.StockSummary)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
