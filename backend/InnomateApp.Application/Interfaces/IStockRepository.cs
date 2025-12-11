using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IStockRepository : IGenericRepository<StockSummary>
    {
        Task<StockSummary?> GetStockSummaryByProductIdAsync(int productId);
        Task UpdateStockSummaryAsync(StockSummary stockSummary);
        Task AddStockTransactionAsync(StockTransaction transaction);
        Task<IReadOnlyList<StockTransaction>> GetStockTransactionsByProductAsync(int productId);
        Task<IReadOnlyList<PurchaseDetail>> GetAvailableBatchesForProductAsync(int productId);
        Task<PurchaseDetail?> GetPurchaseDetailForFifoAsync(int purchaseDetailId);
        Task UpdatePurchaseDetailAsync(PurchaseDetail purchaseDetail);
        Task<PurchaseDetail?> GetPurchaseDetailByIdAsync(int purchaseDetailId);
        Task<IEnumerable<PurchaseDetail>> GetPurchaseDetailsForReturnFIFOAsync(int productId);
    }
}
