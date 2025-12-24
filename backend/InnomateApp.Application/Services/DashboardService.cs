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
                // 1. Fetch Data Sequentially (DbContext is not thread-safe)
                var totalRevenue = await _saleRepo.GetTotalRevenueAsync();
                var totalProfit = await _saleRepo.GetTotalProfitAsync();
                var totalSalesCount = await _saleRepo.GetTotalSalesCountAsync();
                
                var dailyStats = await _saleRepo.GetDailyStatsAsync(today);
                
                var productCount = await _productRepo.CountAsync();
                var lowStockCount = await _productRepo.CountLowStockAsync();
                var customerCount = await _customerRepo.CountAsync();

                var recentSales = await _saleRepo.GetRecentSalesAsync(5);
                var performanceStats = await _saleRepo.GetPerformanceStatsAsync(sevenDaysAgo, today);
                var invoiceStats = await _saleRepo.GetInvoiceStatsAsync();

                // 2. Calculate Trends (Current 7 Days vs Previous 7 Days)
                var previousSevenDaysStart = sevenDaysAgo.AddDays(-7);
                var previousSevenDaysEnd = sevenDaysAgo.AddDays(-1);

                var currentPeriodStats = await _saleRepo.GetPeriodStatsAsync(sevenDaysAgo, today);
                var previousPeriodStats = await _saleRepo.GetPeriodStatsAsync(previousSevenDaysStart, previousSevenDaysEnd);

                // For Customer Growth: New Customers in last 7 days vs Previous 7 days? Or New / TotalBase?
                // Let's use New vs Previous New for "Trend" if the visual is "Since last week"
                // But often "Total" cards show increase in total.
                // If label is "Since last week", it usually implies flow.
                // Let's use: (CurrentNew - PreviousNew) / PreviousNew ? No, that's volatile.
                // Let's use: NewCustomersInPeriod / TotalCustomersBeforePeriod * 100 (Growth rate of base)
                var newCustomersFiles = await _customerRepo.CountAsync(sevenDaysAgo, today);
                var totalCustomersBefore = customerCount - newCustomersFiles;
                double customerGrowth = totalCustomersBefore > 0 
                    ? ((double)newCustomersFiles / totalCustomersBefore) * 100 
                    : (newCustomersFiles > 0 ? 100 : 0);

                // Revenue Growth (Flow comparison)
                double revenueGrowth = previousPeriodStats.Revenue > 0 
                    ? (double)((currentPeriodStats.Revenue - previousPeriodStats.Revenue) / previousPeriodStats.Revenue) * 100
                    : (currentPeriodStats.Revenue > 0 ? 100 : 0);

                // Profit Growth
                double profitGrowth = previousPeriodStats.Profit > 0
                    ? (double)((currentPeriodStats.Profit - previousPeriodStats.Profit) / previousPeriodStats.Profit) * 100
                    : (currentPeriodStats.Profit > 0 ? 100 : 0);
                
                // Invoices Growth
                double invoicesGrowth = previousPeriodStats.Count > 0
                    ? (double)((currentPeriodStats.Count - previousPeriodStats.Count) / (double)previousPeriodStats.Count) * 100
                    : (currentPeriodStats.Count > 0 ? 100 : 0);

                // 3. Construct Response
                var response = new DashboardResponseDto
                {
                    Stats = new DashboardStatsDto
                    {
                        TotalRevenue = totalRevenue,
                        TotalProfit = totalProfit,
                        TotalSalesCount = totalSalesCount,
                        AverageMargin = totalSalesCount > 0 
                            ? (totalProfit / totalRevenue) * 100 
                            : 0,
                        
                        TodayRevenue = dailyStats.Revenue,
                        TodayProfit = dailyStats.Profit,
                        TodaySalesCount = dailyStats.Count,

                        TotalProducts = productCount,
                        LowStockProducts = lowStockCount,
                        TotalCustomers = customerCount,
                        
                        TotalPaidInvoices = invoiceStats.Paid,
                        TotalUnpaidInvoices = invoiceStats.Unpaid, // This is actually "Pending" based on repo logic
                        TotalOverdueInvoices = invoiceStats.Overdue,

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
