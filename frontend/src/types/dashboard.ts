export interface DashboardStats {
    totalRevenue: number;
    totalProfit: number;
    totalSalesCount: number;
    averageMargin: number;
    todayRevenue: number;
    todayProfit: number;
    todaySalesCount: number;
    lowStockProducts: number;
    totalProducts: number;
    totalCustomers: number;
    totalPaidInvoices: number;
    totalUnpaidInvoices: number;
    totalOverdueInvoices: number;
    revenueGrowth: number;
    profitGrowth: number;
    salesCountGrowth: number;
    customersGrowth: number;
}

export interface DashboardChartData {
    date: string;
    revenue: number;
    profit: number;
}

export interface DashboardResponse {
    stats: DashboardStats;
    last7DaysPerformance: DashboardChartData[];
    recentSales: any[]; // Using any for SaleDto to simplify, ideally should import SaleDto
}
