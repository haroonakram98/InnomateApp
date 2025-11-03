using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace InnomateApp.Infrastructure.Repositories
{
    public class StockSummaryRepository : GenericRepository<StockSummary>, IStockSummaryRepository
    {

        public StockSummaryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<StockSummary?> GetByProductIdAsync(int productId)
        {
            return await _context.StockSummaries
                .FirstOrDefaultAsync(s => s.ProductId == productId);
        }

        public async Task<IEnumerable<StockSummary>> GetAllAsync()
        {
            return await _context.StockSummaries.ToListAsync();
        }

        // ✅ Add this
        public async Task DeleteAsync(StockSummary entity)
        {
            _context.StockSummaries.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
