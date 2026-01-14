using InnomateApp.Application.DTOs.Dashboard;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InnomateApp.Infrastructure.Repositories
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext context) : base(context) { }

        public async Task<Sale?> GetSaleWithDetailsAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.UsedBatches) // ✅ Include FIFO batches
                        .ThenInclude(b => b.PurchaseDetail)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.SaleId == id);
        }

        public async Task<IReadOnlyList<Sale>> GetSalesWithDetailsAsync()
        {
            try
            {
                return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.UsedBatches)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
            
        }

        public async Task AddSaleDetailAsync(SaleDetail detail)
        {
            await _context.SaleDetails.AddAsync(detail);
        }

        public async Task UpdateSaleDetailAsync(SaleDetail detail)
        {
            _context.SaleDetails.Update(detail);
        }

        public async Task<SaleDetail?> GetSaleDetailAsync(int saleDetailId)
        {
            return await _context.SaleDetails
                .Include(sd => sd.Product)
                .Include(sd => sd.PurchaseDetail)
                .FirstOrDefaultAsync(sd => sd.SaleDetailId == saleDetailId);
        }

        public async Task<SaleDetail?> GetSaleDetailWithBatchesAsync(int saleDetailId)
        {
            try
            {
                var saleDetail = await _context.SaleDetails
                    .Include(sd => sd.Product) // Include product info
                    .Include(sd => sd.Sale) // Include parent sale
                    .Include(sd => sd.UsedBatches) // ✅ CRITICAL: Include FIFO batches
                        .ThenInclude(b => b.PurchaseDetail) // Include purchase detail for each batch
                            .ThenInclude(pd => pd.Purchase) // Include purchase info
                    .FirstOrDefaultAsync(sd => sd.SaleDetailId == saleDetailId);

                if (saleDetail == null)
                {
                    //_logger.LogWarning("SaleDetail {SaleDetailId} not found", saleDetailId);
                    return null;
                }

                //_logger.LogDebug(
                //    "Retrieved SaleDetail {SaleDetailId}: Product {ProductId}, " +
                //    "Quantity {Quantity}, UsedBatches: {BatchCount}",
                //    saleDetailId,
                //    saleDetail.ProductId,
                //    saleDetail.Quantity,
                //    saleDetail.UsedBatches.Count);

                return saleDetail;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error retrieving SaleDetail {SaleDetailId} with batches",
                //    saleDetailId);
                throw;
            }
        }

        // ========================================================================
        // ✅ BONUS: Additional Useful Methods
        // ========================================================================

        /// <summary>
        /// Gets all sale details for a sale with their FIFO batch information
        /// Useful for bulk operations like cancelling entire sales
        /// </summary>
        public async Task<IEnumerable<SaleDetail>> GetSaleDetailsWithBatchesBySaleIdAsync(int saleId)
        {
            try
            {
                var saleDetails = await _context.SaleDetails
                    .Include(sd => sd.Product)
                    .Include(sd => sd.Sale)
                    .Include(sd => sd.UsedBatches)
                        .ThenInclude(b => b.PurchaseDetail)
                            .ThenInclude(pd => pd.Purchase)
                    .Where(sd => sd.SaleId == saleId)
                    .ToListAsync();

                //_logger.LogDebug(
                //    "Retrieved {Count} SaleDetails for Sale {SaleId}",
                //    saleDetails.Count, saleId);

                return saleDetails;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error retrieving SaleDetails for Sale {SaleId}", saleId);
                throw;
            }
        }

        /// <summary>
        /// Gets a specific sale detail batch record
        /// </summary>
        public async Task<SaleDetailBatch?> GetSaleDetailBatchAsync(int saleDetailBatchId)
        {
            return await _context.SaleDetailBatches
                .Include(b => b.SaleDetail)
                    .ThenInclude(sd => sd.Product)
                .Include(b => b.PurchaseDetail)
                    .ThenInclude(pd => pd.Purchase)
                .FirstOrDefaultAsync(b => b.SaleDetailBatchId == saleDetailBatchId);
        }

        /// <summary>
        /// Gets all batches used in a specific sale
        /// Useful for audit reports showing which purchase batches contributed to a sale
        /// </summary>
        public async Task<IEnumerable<SaleDetailBatch>> GetBatchesUsedInSaleAsync(int saleId)
        {
            return await _context.SaleDetailBatches
                .Include(b => b.SaleDetail)
                    .ThenInclude(sd => sd.Product)
                .Include(b => b.PurchaseDetail)
                    .ThenInclude(pd => pd.Purchase)
                .Where(b => b.SaleDetail.SaleId == saleId)
                .OrderBy(b => b.PurchaseDetail.Purchase!.PurchaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all sales that used a specific purchase batch
        /// Useful for traceability - "which sales consumed this batch?"
        /// </summary>
        public async Task<IEnumerable<SaleDetailBatch>> GetSalesUsingPurchaseBatchAsync(
            int purchaseDetailId)
        {
            return await _context.SaleDetailBatches
                .Include(b => b.SaleDetail)
                    .ThenInclude(sd => sd.Sale)
                        .ThenInclude(s => s.Customer)
                .Include(b => b.PurchaseDetail)
                .Where(b => b.PurchaseDetailId == purchaseDetailId)
                .OrderBy(b => b.SaleDetail.Sale.SaleDate)
                .ToListAsync();
        }
        public async Task<string?> GetLastInvoiceNoAsync()
        {
            return await _context.Sales
                .OrderByDescending(s => s.SaleId)
                .Select(s => s.InvoiceNo)
                .FirstOrDefaultAsync();
        }

        public async Task<DashboardAggregates> GetDashboardAggregatesAsync()
        {
            var now = DateTime.Now;
            var todayStart = now.Date;
            var todayEnd = todayStart.AddDays(1).AddTicks(-1);

            // 7 Days Range (Current Period)
            var sevenDaysAgo = todayStart.AddDays(-6);
             // Previous 7 Days (Previous Period)
            var prevSevenDaysStart = sevenDaysAgo.AddDays(-7);
            var prevSevenDaysEnd = sevenDaysAgo.AddDays(-1).AddDays(1).AddTicks(-1); // End of the day before current period starts

            // 30 Days Ago (Invoice Stats)
            var thirtyDaysAgo = todayStart.AddDays(-30);

            var query = _context.Sales.Where(s => !s.IsDeleted);

            var stats = await query
                .GroupBy(s => 1)
                .Select(g => new
                {
                    // Total Lifetime
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalProfit = g.Sum(x => x.TotalProfit),
                    TotalCount = g.Count(),

                    // Today
                    TodayRevenue = g.Where(x => x.SaleDate >= todayStart && x.SaleDate <= todayEnd).Sum(x => x.TotalAmount),
                    TodayProfit = g.Where(x => x.SaleDate >= todayStart && x.SaleDate <= todayEnd).Sum(x => x.TotalProfit),
                    TodayCount = g.Where(x => x.SaleDate >= todayStart && x.SaleDate <= todayEnd).Count(),

                    // Current Period (Last 7 Days) - For Growth Calc
                    CurrentPeriodRevenue = g.Where(x => x.SaleDate >= sevenDaysAgo && x.SaleDate <= now).Sum(x => x.TotalAmount),
                    CurrentPeriodProfit = g.Where(x => x.SaleDate >= sevenDaysAgo && x.SaleDate <= now).Sum(x => x.TotalProfit),
                    CurrentPeriodCount = g.Where(x => x.SaleDate >= sevenDaysAgo && x.SaleDate <= now).Count(),

                    // Previous Period (Prior 7 Days) - For Growth Calc
                    PrevPeriodRevenue = g.Where(x => x.SaleDate >= prevSevenDaysStart && x.SaleDate <= prevSevenDaysEnd).Sum(x => x.TotalAmount),
                    PrevPeriodProfit = g.Where(x => x.SaleDate >= prevSevenDaysStart && x.SaleDate <= prevSevenDaysEnd).Sum(x => x.TotalProfit),
                    PrevPeriodCount = g.Where(x => x.SaleDate >= prevSevenDaysStart && x.SaleDate <= prevSevenDaysEnd).Count(),

                    // Invoices
                    PaidInvoices = g.Count(x => x.IsFullyPaid),
                    OverdueInvoices = g.Count(x => !x.IsFullyPaid && x.SaleDate < thirtyDaysAgo),
                    PendingInvoices = g.Count(x => !x.IsFullyPaid && x.SaleDate >= thirtyDaysAgo)
                })
                .FirstOrDefaultAsync();

            var result = new DashboardAggregates
            {
                TotalRevenue = stats?.TotalRevenue ?? 0,
                TotalProfit = stats?.TotalProfit ?? 0,
                TotalSalesCount = stats?.TotalCount ?? 0,
                TodayRevenue = stats?.TodayRevenue ?? 0,
                TodayProfit = stats?.TodayProfit ?? 0,
                TodaySalesCount = stats?.TodayCount ?? 0,
                
                CurrentPeriodRevenue = stats?.CurrentPeriodRevenue ?? 0,
                CurrentPeriodProfit = stats?.CurrentPeriodProfit ?? 0,
                CurrentPeriodCount = stats?.CurrentPeriodCount ?? 0,
                
                PrevPeriodRevenue = stats?.PrevPeriodRevenue ?? 0,
                PrevPeriodProfit = stats?.PrevPeriodProfit ?? 0,
                PrevPeriodCount = stats?.PrevPeriodCount ?? 0,

                PaidInvoices = stats?.PaidInvoices ?? 0,
                OverdueInvoices = stats?.OverdueInvoices ?? 0,
                PendingInvoices = stats?.PendingInvoices ?? 0,

                // We will populate Products/Customer in service
                ProductCount = 0,
                CustomerCount = 0,
                LowStockCount = 0
            };

            // Calculate Growth Percentages inside Service or here? 
            // Better to return the raw period data to Service, but DTO doesn't have fields for "PrevPeriodRevenue".
            // I should have added them to DTO or created a specific DTO.
            // For now, I will calculate growth HERE and stuffing it into the DTO? 
            // The DTO has "RevenueGrowth" property? No, DTO has `TodayRevenue`. 
            // Wait, `DashboardAggregates` DTO (from Step 78) does NOT have Growth fields!
            // `DashboardStatsDto` (in DashboardService.cs) DOES.
            // My new DTO `DashboardAggregates` (Step 78) matches `DashboardStatsDto` structure PARTIALLY.
            
            // Re-checking `DashboardAggregates.cs` content I created:
            // It has Total..., Today..., ProductCount...
            // It does NOT have Growth fields.

            // So I can't return growth from here unless I modify DTO.
            // I will modify DTO to include `CurrentPeriodRevenue`, `PrevPeriodRevenue` etc.
            // OR I will calculate it in Service if I can't get it out.
            // I'll modify the DTO in `DashboardAggregates.cs` quickly to include Period Stats so Service can calc growth.
            // Actually, I'll just add `PeriodStats` to `DashboardAggregates`?
            // To be safe and clean, I will update `DashboardAggregates` to include these values.
            return result;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            // Sum of TotalAmount for all non-deleted sales
            return await _context.Sales
                .Where(s => !s.IsDeleted)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<decimal> GetTotalProfitAsync()
        {
            // Sum of TotalProfit for all non-deleted sales
            return await _context.Sales
                .Where(s => !s.IsDeleted)
                .SumAsync(s => s.TotalProfit);
        }

        public async Task<int> GetTotalSalesCountAsync()
        {
            return await _context.Sales.Where(s => !s.IsDeleted).CountAsync();
        }

        public async Task<(decimal Revenue, decimal Profit, int Count)> GetDailyStatsAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            var dailyStats = await _context.Sales
                .Where(s => !s.IsDeleted && s.SaleDate >= startOfDay && s.SaleDate <= endOfDay)
                .GroupBy(s => 1)
                .Select(g => new
                {
                    Revenue = g.Sum(s => s.TotalAmount),
                    Profit = g.Sum(s => s.TotalProfit),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            return (dailyStats?.Revenue ?? 0, dailyStats?.Profit ?? 0, dailyStats?.Count ?? 0);
        }
        
        public async Task<IReadOnlyList<Sale>> GetRecentSalesAsync(int count)
        {
             return await _context.Sales
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.SaleDate)
                .Take(count)
                .Include(s => s.Customer)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<IEnumerable<(DateTime Date, decimal Revenue, decimal Profit)>> GetPerformanceStatsAsync(DateTime startDate, DateTime endDate)
        {
            var stats = await _context.Sales
                .Where(s => !s.IsDeleted && s.SaleDate >= startDate && s.SaleDate <= endDate)
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(s => s.TotalAmount),
                    Profit = g.Sum(s => s.TotalProfit)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return stats.Select(x => (x.Date, x.Revenue, x.Profit));
        }

        public async Task<(int Paid, int Unpaid, int Overdue)> GetInvoiceStatsAsync()
        {
            var today = DateTime.Now;
            var thirtyDaysAgo = today.AddDays(-30);

            var paid = await _context.Sales.Where(s => !s.IsDeleted && s.IsFullyPaid).CountAsync();
            
            // Unpaid logic: Not fully paid
            // We can split Unpaid into "Overdue" (older than 30 days) and "Pending" (newer)
            // Or "Unpaid" represents ALL unpaid, and "Overdue" is a subset?
            // Usually in charts: Paid + Unpaid + Overdue should sum up to Total, OR they are distinct categories.
            // If the chart is a pie chart of 3 slices, they must be distinct.
            // So:
            // Paid = Fully Paid
            // Overdue = Not Fully Paid AND SaleDate < 30 days ago
            // Unpaid (Pending) = Not Fully Paid AND SaleDate >= 30 days ago
            
            var overdue = await _context.Sales
                .Where(s => !s.IsDeleted && !s.IsFullyPaid && s.SaleDate < thirtyDaysAgo)
                .CountAsync();

            var pending = await _context.Sales
                .Where(s => !s.IsDeleted && !s.IsFullyPaid && s.SaleDate >= thirtyDaysAgo)
                .CountAsync();

            return (paid, pending, overdue);
        }

        public async Task<(decimal Revenue, decimal Profit, int Count)> GetPeriodStatsAsync(DateTime startDate, DateTime endDate)
        {
            // Adjust endDate to include the full day
            var actualEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            var stats = await _context.Sales
                .Where(s => !s.IsDeleted && s.SaleDate >= startDate.Date && s.SaleDate <= actualEndDate)
                .GroupBy(s => 1)
                .Select(g => new
                {
                    Revenue = g.Sum(s => s.TotalAmount),
                    Profit = g.Sum(s => s.TotalProfit),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            return (stats?.Revenue ?? 0, stats?.Profit ?? 0, stats?.Count ?? 0);
        }
    }
}
