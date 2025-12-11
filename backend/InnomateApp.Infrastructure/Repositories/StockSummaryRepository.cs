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
            return await _context.StockSummaries.Include(ss => ss.Product).ToListAsync();
        }

        // ✅ Add this
        public async Task DeleteAsync(StockSummary entity)
        {
            _context.StockSummaries.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateStockSummaryAsync(int productId, decimal quantity, decimal unitCost)
        {
            var stockSummary = await GetByProductIdAsync(productId);

            if (stockSummary == null)
            {
                stockSummary = new StockSummary
                {
                    ProductId = productId,
                    TotalIn = quantity,
                    TotalOut = 0,
                    Balance = quantity,
                    AverageCost = unitCost,
                    TotalValue = quantity * unitCost,
                    LastUpdated = DateTime.Now
                };
                await _context.StockSummaries.AddAsync(stockSummary);
            }
            else
            {
                // Calculate weighted average cost
                var totalQuantity = stockSummary.Balance + quantity;
                var totalValue = (stockSummary.Balance * stockSummary.AverageCost) + (quantity * unitCost);

                stockSummary.TotalIn += quantity;
                stockSummary.Balance = totalQuantity;
                stockSummary.AverageCost = totalValue / totalQuantity;
                stockSummary.TotalValue = totalValue;
                stockSummary.LastUpdated = DateTime.Now;

                _context.StockSummaries.Update(stockSummary);
            }

            await _context.SaveChangesAsync();
        }
    }
}
