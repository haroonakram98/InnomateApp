using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<Purchase> CreatePurchaseAsync(Purchase purchase);
        Task<Purchase> GetPurchaseByIdAsync(int purchaseId);
        Task<Purchase> ReceivePurchaseAsync(int purchaseId);
        Task<bool> CancelPurchaseAsync(int purchaseId);
        Task<IEnumerable<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IReadOnlyList<Purchase>> GetPurchasesWithDetailsAsync();
        Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId);
        Task<string> GetNextPurchaseNumberAsync();
        Task<string> GetNextBatchNumberAsync();
    }
}
