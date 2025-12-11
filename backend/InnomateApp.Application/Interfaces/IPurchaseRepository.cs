using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IPurchaseRepository : IGenericRepository<Purchase>
    {
        Task<Purchase?> GetPurchaseWithDetailsAsync(int id);
        Task<IReadOnlyList<Purchase>> GetPurchasesWithDetailsAsync();
        Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId);
        Task<PurchaseDetail?> GetPurchaseDetailAsync(int purchaseDetailId);
        Task AddPurchaseDetailAsync(PurchaseDetail detail);
        Task UpdatePurchaseDetailAsync(PurchaseDetail detail);
        Task<IReadOnlyList<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<int, decimal>> GetProductCostsAsync(List<int> productIds);
    }
}
