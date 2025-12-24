using System.Collections.Generic;

namespace InnomateApp.Application.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalSalesCount { get; set; }
        public decimal AverageMargin { get; set; }
        
        // Today's stats
        public decimal TodayRevenue { get; set; }
        public decimal TodayProfit { get; set; }
        public int TodaySalesCount { get; set; }

        // Inventory & Customers
        public int LowStockProducts { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }

        // Invoice Stats
        public int TotalPaidInvoices { get; set; }
        public int TotalUnpaidInvoices { get; set; }
        public int TotalOverdueInvoices { get; set; }

        // Trends (Percentage Growth)
        public double RevenueGrowth { get; set; }
        public double ProfitGrowth { get; set; }
        public double SalesCountGrowth { get; set; }
        public double CustomersGrowth { get; set; }
    }

    public class DashboardChartDataDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
    }

    public class DashboardResponseDto
    {
        public DashboardStatsDto Stats { get; set; } = new();
        public IEnumerable<DashboardChartDataDto> Last7DaysPerformance { get; set; } = new List<DashboardChartDataDto>();
        public IEnumerable<SaleDto> RecentSales { get; set; } = new List<SaleDto>();
    }
}
