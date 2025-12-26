using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            ISaleRepository saleRepo,
            IProductRepository productRepo,
            ICustomerRepository customerRepo,
            IMapper mapper,
            ILogger<DashboardService> logger)
        {
            _saleRepo = saleRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DashboardResponseDto> GetDashboardStatsAsync()
        {
            var today = DateTime.Now;
            var sevenDaysAgo = today.AddDays(-6); // Include today

            try
            {
                // ✅ OPTIMIZED: Single aggregated query for all sales-related stats
                var aggregates = await _saleRepo.GetDashboardAggregatesAsync();

                // Fetch remaining counts (Product, Customer, LowStock) - 3 additional queries
                var productCount = await _productRepo.CountAsync();
                var lowStockCount = await _productRepo.CountLowStockAsync();
                var customerCount = await _customerRepo.CountAsync();

                // Update aggregates with product/customer data
                aggregates.ProductCount = productCount;
                aggregates.LowStockCount = lowStockCount;
                aggregates.CustomerCount = customerCount;

                // Fetch recent sales and performance stats (still needed for detailed data)
                var recentSales = await _saleRepo.GetRecentSalesAsync(5);
                var performanceStats = await _saleRepo.GetPerformanceStatsAsync(sevenDaysAgo, today);

                // Calculate customer growth
                var newCustomersInPeriod = await _customerRepo.CountAsync(sevenDaysAgo, today);
                var totalCustomersBefore = customerCount - newCustomersInPeriod;
                double customerGrowth = totalCustomersBefore > 0 
                    ? ((double)newCustomersInPeriod / totalCustomersBefore) * 100 
                    : (newCustomersInPeriod > 0 ? 100 : 0);

                // Calculate growth percentages from aggregated period data
                double revenueGrowth = aggregates.PrevPeriodRevenue > 0 
                    ? (double)((aggregates.CurrentPeriodRevenue - aggregates.PrevPeriodRevenue) / aggregates.PrevPeriodRevenue) * 100
                    : (aggregates.CurrentPeriodRevenue > 0 ? 100 : 0);

                double profitGrowth = aggregates.PrevPeriodProfit > 0
                    ? (double)((aggregates.CurrentPeriodProfit - aggregates.PrevPeriodProfit) / aggregates.PrevPeriodProfit) * 100
                    : (aggregates.CurrentPeriodProfit > 0 ? 100 : 0);
                
                double invoicesGrowth = aggregates.PrevPeriodCount > 0
                    ? (double)((aggregates.CurrentPeriodCount - aggregates.PrevPeriodCount) / (double)aggregates.PrevPeriodCount) * 100
                    : (aggregates.CurrentPeriodCount > 0 ? 100 : 0);

                // Construct Response
                var response = new DashboardResponseDto
                {
                    Stats = new DashboardStatsDto
                    {
                        TotalRevenue = aggregates.TotalRevenue,
                        TotalProfit = aggregates.TotalProfit,
                        TotalSalesCount = aggregates.TotalSalesCount,
                        AverageMargin = aggregates.TotalSalesCount > 0 
                            ? (aggregates.TotalProfit / aggregates.TotalRevenue) * 100 
                            : 0,
                        
                        TodayRevenue = aggregates.TodayRevenue,
                        TodayProfit = aggregates.TodayProfit,
                        TodaySalesCount = aggregates.TodaySalesCount,

                        TotalProducts = aggregates.ProductCount,
                        LowStockProducts = aggregates.LowStockCount,
                        TotalCustomers = aggregates.CustomerCount,
                        
                        TotalPaidInvoices = aggregates.PaidInvoices,
                        TotalUnpaidInvoices = aggregates.PendingInvoices, // "Pending" invoices
                        TotalOverdueInvoices = aggregates.OverdueInvoices,

                        RevenueGrowth = Math.Round(revenueGrowth, 1),
                        ProfitGrowth = Math.Round(profitGrowth, 1),
                        SalesCountGrowth = Math.Round(invoicesGrowth, 1),
                        CustomersGrowth = Math.Round(customerGrowth, 1)
                    },
                    RecentSales = _mapper.Map<IEnumerable<SaleDto>>(recentSales),
                    Last7DaysPerformance = performanceStats.Select(p => new DashboardChartDataDto
                    {
                        Date = p.Date.ToString("MMM dd"),
                        Revenue = p.Revenue,
                        Profit = p.Profit
                    })
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dashboard stats.");
                throw; // Rethrow to let the controller handle 500 or global handler
            }
        }
    }
}
