using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IStockTransactionRepository : IGenericRepository<StockTransaction>
    {
        Task<IReadOnlyList<StockTransaction>> GetTransactionsByProductAsync(int productId);
        Task<IReadOnlyList<StockTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
