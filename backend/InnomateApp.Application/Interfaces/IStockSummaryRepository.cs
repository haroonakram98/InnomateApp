using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IStockSummaryRepository : IGenericRepository<StockSummary>
    {
        Task<StockSummary?> GetByProductIdAsync(int productId);
        Task<IEnumerable<StockSummary>> GetAllAsync();
        Task DeleteAsync(StockSummary entity); // Add this
    }
}
