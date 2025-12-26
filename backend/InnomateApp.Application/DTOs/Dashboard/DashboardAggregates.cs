using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.DTOs.Dashboard
{
    public class DashboardAggregates
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalSalesCount { get; set; }
        
        // Today's stats
        public decimal TodayRevenue { get; set; }
        public decimal TodayProfit { get; set; }
        public int TodaySalesCount { get; set; }

        // Counts
        public int ProductCount { get; set; }
        public int LowStockCount { get; set; }
        public int CustomerCount { get; set; }

        // Growth Calculation Data
        public decimal CurrentPeriodRevenue { get; set; }
        public decimal CurrentPeriodProfit { get; set; }
        public int CurrentPeriodCount { get; set; }

        public decimal PrevPeriodRevenue { get; set; }
        public decimal PrevPeriodProfit { get; set; }
        public int PrevPeriodCount { get; set; }

        // Invoice Stats
        public int PaidInvoices { get; set; }
        public int OverdueInvoices { get; set; }
        public int PendingInvoices { get; set; }
    }
}
