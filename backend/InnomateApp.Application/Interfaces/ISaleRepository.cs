using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        Task<Sale?> GetSaleWithDetailsAsync(int id);
        Task<IReadOnlyList<Sale>> GetSalesWithDetailsAsync();
        Task AddSaleDetailAsync(SaleDetail detail);
        Task UpdateSaleDetailAsync(SaleDetail detail);
        Task<SaleDetail?> GetSaleDetailAsync(int saleDetailId);
        Task<SaleDetail?> GetSaleDetailWithBatchesAsync(int saleDetailId);

        // ✅ BONUS: Additional useful methods
        Task<IEnumerable<SaleDetail>> GetSaleDetailsWithBatchesBySaleIdAsync(int saleId);
        Task<SaleDetailBatch?> GetSaleDetailBatchAsync(int saleDetailBatchId);
        Task<string?> GetLastInvoiceNoAsync();
    }
}
