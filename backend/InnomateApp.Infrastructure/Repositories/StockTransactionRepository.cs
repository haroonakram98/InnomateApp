using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Repositories
{
    public class StockTransactionRepository : GenericRepository<StockTransaction>, IStockTransactionRepository
    {
        public StockTransactionRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<StockTransaction>> GetTransactionsByProductAsync(int productId)
        {
            return await _context.StockTransactions
                .Where(st => st.ProductId == productId)
                .OrderByDescending(st => st.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<StockTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.StockTransactions
                .Include(st => st.Product)
                .Where(st => st.CreatedAt >= startDate && st.CreatedAt <= endDate)
                .OrderByDescending(st => st.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
