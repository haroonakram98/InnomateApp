using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Category operations
    /// </summary>
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Category?> GetByNameAsync(string name, int tenantId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => 
                    c.Name.ToLower() == name.ToLower() && 
                    c.TenantId == tenantId);
        }

        /// <inheritdoc />
        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _context.Products
                .CountAsync(p => p.CategoryId == categoryId && p.IsActive);
        }

        /// <inheritdoc />
        public async Task<List<CategoryResponse>> GetAllWithProductCountAsync(int tenantId)
        {
            return await _context.Categories
                .Where(c => c.TenantId == tenantId)
                .Select(c => new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count(p => p.IsActive)
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<CategoryLookupResponse>> GetLookupAsync(int tenantId)
        {
            return await _context.Categories
                .Where(c => c.TenantId == tenantId)
                .Select(c => new CategoryLookupResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}

