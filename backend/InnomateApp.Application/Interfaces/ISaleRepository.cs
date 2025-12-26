using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using InnomateApp.Application.DTOs.Dashboard;

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

        // Dashboard Stats
        Task<DashboardAggregates> GetDashboardAggregatesAsync(); // ✅ Optimized Single Call
        
        Task<decimal> GetTotalRevenueAsync(); // Kept for legacy if needed, or remove? I'll keep for now.
        Task<decimal> GetTotalProfitAsync();
        Task<int> GetTotalSalesCountAsync();
        Task<(decimal Revenue, decimal Profit, int Count)> GetDailyStatsAsync(DateTime date);
        Task<IReadOnlyList<Sale>> GetRecentSalesAsync(int count);
        Task<IEnumerable<(DateTime Date, decimal Revenue, decimal Profit)>> GetPerformanceStatsAsync(DateTime startDate, DateTime endDate);
        Task<(int Paid, int Unpaid, int Overdue)> GetInvoiceStatsAsync();
        Task<(decimal Revenue, decimal Profit, int Count)> GetPeriodStatsAsync(DateTime startDate, DateTime endDate);
    }
}
