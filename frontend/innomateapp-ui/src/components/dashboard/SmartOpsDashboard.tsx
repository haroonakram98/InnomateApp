import React, { useEffect, useState } from "react";
import { Card, CardContent } from "@/components/ui/Card.js";
import Button from "@/components/ui/Button.js";
import {
  Users,
  DollarSign,
  FileText,
  Search,
  MessageSquare,
  Bell,
  ChevronDown,
  Filter,
  MoreHorizontal,
  Wallet,
  PieChart as PieChartIcon,
  CreditCard
} from "lucide-react";
import AddPaymentModal from "@/components/sales/AddPaymentModal.js";
import { dashboardService } from "@/services/dashboardService.js";
import { saleService } from "@/services/saleService.js";
import { DashboardResponse } from "@/types/dashboard.js";
import { SaleDTO } from "@/types/sale.js";
import { useToastStore } from "@/store/useToastStore.js";
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Legend,
  Sector
} from "recharts";

const SmartOpsDashboard: React.FC = () => {
  const [data, setData] = useState<DashboardResponse | null>(null);
  const [sales, setSales] = useState<SaleDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedSale, setSelectedSale] = useState<SaleDTO | null>(null);
  const [isPaymentModalOpen, setIsPaymentModalOpen] = useState(false);
  const toast = useToastStore(state => state.push);

  const fetchData = async () => {
    setLoading(true);
    try {
      const stats = await dashboardService.getStats();
      setData(stats);
      const salesData = await saleService.getAll();
      setSales(salesData);
    } catch (error) {
      console.error(error);
      toast("Failed to load dashboard data", 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-PK', {
      style: 'currency',
      currency: 'PKR',
      maximumFractionDigits: 0
    }).format(amount);
  };

  // Data reordered to match visual rotation: Top-Right (Paid), Bottom-Right (Unpaid), Left (Overdue)
  const invoiceStatsData = [
    { name: 'Total Paid', value: data?.stats?.totalPaidInvoices || 0, color: '#1e293b' }, // Dark/Black
    { name: 'Total Unpaid', value: data?.stats?.totalUnpaidInvoices || 0, color: '#e2e8f0' }, // Light Grey
    { name: 'Total Overdue', value: data?.stats?.totalOverdueInvoices || 0, color: '#6366f1' }, // Indigo/Blue
  ];

  const RADIAN = Math.PI / 180;

  const renderActiveShape = (props: any) => {
    const { cx, cy, innerRadius, outerRadius, startAngle, endAngle, fill } = props;
    return (
      <g>
        <Sector
          cx={cx}
          cy={cy}
          innerRadius={innerRadius}
          outerRadius={outerRadius + 8}
          startAngle={startAngle}
          endAngle={endAngle}
          fill={fill}
        />
      </g>
    );
  };

  const renderCustomizedLabel = ({ cx, cy, midAngle, innerRadius, outerRadius, percent, index }: any) => {
    // For active slice (index 0), use the expanded radius for label calculation or keep it centered?
    // User image shows label inside. Standard calculation usually works fine.
    // However, if the slice is bigger, the standard label position might be slightly off center of the new mass,
    // but usually 'outerRadius' passed to this function is the standard one unless Recharts passes the active one.
    // Recharts passes the *standard* props to label function even if active.
    // Let's adjust radius for index 0 to centering it better in the larger slice if needed.
    const currentOuterRadius = index === 0 ? outerRadius + 8 : outerRadius;
    const radius = innerRadius + (currentOuterRadius - innerRadius) * 0.5;
    const x = cx + radius * Math.cos(-midAngle * RADIAN);
    const y = cy + radius * Math.sin(-midAngle * RADIAN);
    const textColor = index === 1 ? '#64748b' : 'white';

    return (
      <text x={x} y={y} fill={textColor} textAnchor="middle" dominantBaseline="central" fontSize={11} fontWeight="bold">
        {`${(percent * 100).toFixed(1)}%`}
      </text>
    );
  };

  // --- Mock Data for "Status" usage in Recent Sales ---
  // We'll randomly assign statuses if not present, just for visual parity with the design
  const getStatusVariant = (id: number) => {
    const statuses = ['Paid', 'Pending', 'Overdue'];
    const val = statuses[id % 3];
    switch (val) {
      case 'Paid': return 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400';
      case 'Pending': return 'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400';
      case 'Overdue': return 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400';
      default: return 'bg-gray-100 text-gray-700';
    }
  };
  const getStatusLabel = (id: number) => ['Paid', 'Pending', 'Overdue'][id % 3];


  if (!data && loading) return (
    <div className="flex items-center justify-center h-96">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
    </div>
  );

  return (
    <div className="flex flex-col p-6 space-y-6 font-sans text-slate-800 dark:text-slate-100">

      {/* 1. Header Section */}
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
        <div>
          <h1 className="text-xl md:text-2xl font-bold text-slate-900 dark:text-white">
            Welcome Back, Admin 👋
          </h1>
          <p className="text-sm text-slate-500 dark:text-slate-400 mt-1">
            Here's what's happening with your store today.
          </p>
        </div>

        <div className="flex items-center gap-3 bg-white dark:bg-slate-800 p-1.5 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 w-full md:w-auto overflow-x-auto">
          <div className="relative hidden lg:block">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              placeholder="Search Anything"
              className="pl-9 pr-4 py-1.5 bg-slate-50 dark:bg-slate-900 rounded-lg text-xs focus:outline-none focus:ring-2 focus:ring-indigo-100 dark:focus:ring-indigo-900 transition-all w-56"
            />
          </div>
          <div className="h-5 w-px bg-slate-200 dark:bg-slate-700 mx-1 hidden lg:block"></div>
          <button className="p-1.5 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-lg text-slate-500 relative transition-colors flex-shrink-0">
            <MessageSquare size={18} />
          </button>
          <button className="p-1.5 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-lg text-slate-500 relative transition-colors flex-shrink-0">
            <Bell size={18} />
            <span className="absolute top-1.5 right-1.5 w-1.5 h-1.5 bg-red-500 rounded-full border-2 border-white dark:border-slate-800"></span>
          </button>
        </div>
      </div>

      {/* 2. Key Stats Cards Row */}
      {/* 2. Key Stats Cards Row */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {[
          {
            label: 'Customers',
            value: data?.stats?.totalCustomers || 0,
            icon: Users,
            barColor: 'bg-indigo-600',
            iconColor: 'text-indigo-600',
            iconBg: 'bg-indigo-50 dark:bg-indigo-900/20',
            trend: data ? `${(data.stats.customersGrowth || 0) > 0 ? '↑' : ((data.stats.customersGrowth || 0) < 0 ? '↓' : '')} ${(data.stats.customersGrowth || 0) > 0 ? '+' : ''}${data.stats.customersGrowth || 0}%` : '-',
            trendColor: (data?.stats?.customersGrowth || 0) >= 0 ? 'text-emerald-500' : 'text-rose-500',
            trendLabel: 'Since last week'
          },
          {
            label: 'Revenue',
            value: data ? formatCurrency(data.stats.totalRevenue) : "-",
            icon: Wallet,
            barColor: 'bg-emerald-500',
            iconColor: 'text-emerald-500',
            iconBg: 'bg-emerald-50 dark:bg-emerald-900/20',
            trend: data ? `${(data.stats.revenueGrowth || 0) > 0 ? '↑' : ((data.stats.revenueGrowth || 0) < 0 ? '↓' : '')} ${(data.stats.revenueGrowth || 0) > 0 ? '+' : ''}${data.stats.revenueGrowth || 0}%` : '-',
            trendColor: (data?.stats?.revenueGrowth || 0) >= 0 ? 'text-emerald-500' : 'text-rose-500',
            trendLabel: 'Since last week'
          },
          {
            label: 'Profit',
            value: data ? formatCurrency(data.stats.totalProfit) : "-",
            icon: PieChartIcon,
            barColor: 'bg-purple-600',
            iconColor: 'text-purple-600',
            iconBg: 'bg-purple-50 dark:bg-purple-900/20',
            trend: data ? `${(data.stats.profitGrowth || 0) > 0 ? '↑' : ((data.stats.profitGrowth || 0) < 0 ? '↓' : '')} ${(data.stats.profitGrowth || 0) > 0 ? '+' : ''}${data.stats.profitGrowth || 0}%` : '-',
            trendColor: (data?.stats?.profitGrowth || 0) >= 0 ? 'text-emerald-500' : 'text-rose-500',
            trendLabel: 'Since last week'
          },
          {
            label: 'Invoices',
            value: data?.stats?.totalSalesCount || 0,
            icon: FileText,
            barColor: 'bg-blue-500',
            iconColor: 'text-blue-500',
            iconBg: 'bg-blue-50 dark:bg-blue-900/20',
            trend: data ? `${(data.stats.salesCountGrowth || 0) > 0 ? '↑' : ((data.stats.salesCountGrowth || 0) < 0 ? '↓' : '')} ${(data.stats.salesCountGrowth || 0) > 0 ? '+' : ''}${data.stats.salesCountGrowth || 0}%` : '-',
            trendColor: (data?.stats?.salesCountGrowth || 0) >= 0 ? 'text-emerald-500' : 'text-rose-500',
            trendLabel: 'Since last week'
          }
        ].map((card, index) => (
          <div key={index} className="bg-white dark:bg-slate-800 rounded-xl p-5 shadow-sm border border-slate-100 dark:border-slate-700 hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div className="flex gap-4 items-center">
                <div className={`w-1.5 h-12 rounded-full ${card.barColor}`}></div>
                <div>
                  <p className="text-sm text-slate-500 dark:text-slate-400 font-medium mb-0.5">{card.label}</p>
                  <h3 className="text-3xl font-bold text-slate-800 dark:text-white leading-none">
                    {card.value}
                  </h3>
                </div>
              </div>
              <div className={`p-3 rounded-2xl ${card.iconBg} ${card.iconColor}`}>
                <card.icon size={24} />
              </div>
            </div>
            <div className="flex items-center text-sm font-medium pl-[1.375rem]">
              <span className={`${card.trendColor}`}>{card.trend}</span>
              <span className="text-slate-400 font-normal ml-2">{card.trendLabel}</span>
            </div>
          </div>
        ))}
      </div>

      {/* 3. Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        {/* Invoice Statistics (Donut/Pie) */}
        <div className="lg:col-span-1 bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 p-6 flex flex-col min-w-0">
          <div className="flex justify-between items-start mb-2">
            <h3 className="font-bold text-base text-slate-700 dark:text-white">Invoice Statistics</h3>
            <button className="text-slate-400 hover:text-slate-600"><MoreHorizontal size={18} /></button>
          </div>

          <div className="flex flex-1 items-center">
            {/* Left: Chart */}
            <div className="flex-1 relative h-[200px] min-w-[160px]">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={invoiceStatsData}
                    cx="50%"
                    cy="50%"
                    innerRadius={55}
                    outerRadius={75}
                    paddingAngle={0}
                    dataKey="value"
                    startAngle={90}
                    endAngle={-270}
                    labelLine={false}
                    label={renderCustomizedLabel}
                    shape={(props: any) => {
                      if (props.index === 0) return renderActiveShape(props);
                      return (
                        <Sector
                          cx={props.cx}
                          cy={props.cy}
                          innerRadius={props.innerRadius}
                          outerRadius={props.outerRadius}
                          startAngle={props.startAngle}
                          endAngle={props.endAngle}
                          fill={props.fill}
                        />
                      );
                    }}
                  >
                    {invoiceStatsData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} strokeWidth={0} />
                    ))}
                  </Pie>
                  <Tooltip
                    contentStyle={{ borderRadius: '12px', border: 'none', boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}
                  />
                </PieChart>
              </ResponsiveContainer>
              {/* Center Text Overlay */}
              <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
                <span className="text-2xl font-bold text-slate-800 dark:text-white">
                  {data?.stats?.totalSalesCount || 0}
                </span>
                <span className="text-xs text-slate-400 font-medium">Invoices</span>
              </div>
            </div>

            {/* Right: Custom Legend/Stats */}
            <div className="flex-1 flex flex-col justify-center gap-6 pl-4">
              {/* Total Paid */}
              <div>
                <p className="text-slate-400 text-xs font-medium mb-1">Total Paid</p>
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full bg-slate-800 dark:bg-slate-200"></div>
                  <span className="text-2xl font-bold text-slate-800 dark:text-white">
                    {data?.stats?.totalPaidInvoices || 0}
                  </span>
                </div>
              </div>

              {/* Total Overdue */}
              <div>
                <p className="text-slate-400 text-xs font-medium mb-1">Total Overdue</p>
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full bg-indigo-500"></div>
                  <span className="text-2xl font-bold text-slate-800 dark:text-white">
                    {data?.stats?.totalOverdueInvoices || 0}
                  </span>
                </div>
              </div>

              {/* Total Unpaid */}
              <div>
                <p className="text-slate-400 text-xs font-medium mb-1">Total Unpaid</p>
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full bg-slate-200 dark:bg-slate-600"></div>
                  <span className="text-2xl font-bold text-slate-800 dark:text-white">
                    {data?.stats?.totalUnpaidInvoices || 0}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Sales Analytics (Area/Line Chart) */}
        <div className="lg:col-span-2 bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 p-6 min-w-0">
          <div className="flex justify-between items-center mb-4">
            <h3 className="font-bold text-base text-slate-800 dark:text-white">Sales Analytics</h3>
            <div className="flex gap-2">
              <select className="bg-slate-50 dark:bg-slate-900 border-none text-xs rounded-lg py-1 px-2 text-slate-600 focus:ring-0 cursor-pointer">
                <option>Weekly</option>
                <option>Monthly</option>
              </select>
              <button className="text-slate-400 hover:text-slate-600"><MoreHorizontal size={18} /></button>
            </div>
          </div>
          <div className="h-[240px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart
                data={data?.last7DaysPerformance || []}
                margin={{ top: 5, right: 5, left: -10, bottom: 0 }}
              >
                <defs>
                  <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#6366f1" stopOpacity={0.2} />
                    <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#E2E8F0" />
                <XAxis
                  dataKey="date"
                  axisLine={false}
                  tickLine={false}
                  tick={{ fill: '#94A3B8', fontSize: 10 }}
                  dy={10}
                />
                <YAxis
                  axisLine={false}
                  tickLine={false}
                  tick={{ fill: '#94A3B8', fontSize: 10 }}
                  tickFormatter={(value) => `${value / 1000}k`}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: '#1F2937',
                    color: '#F9FAFB',
                    borderRadius: '8px',
                    border: 'none',
                    boxShadow: '0 4px 20px rgba(0,0,0,0.2)',
                    fontSize: '12px',
                    padding: '8px 12px'
                  }}
                  itemStyle={{ color: '#F9FAFB' }}
                  formatter={(value: number | undefined) => [formatCurrency(value || 0), 'Revenue']}
                />
                <Area
                  type="monotone"
                  dataKey="revenue"
                  stroke="#6366f1"
                  strokeWidth={2}
                  fillOpacity={1}
                  fill="url(#colorRevenue)"
                  dot={{ r: 3, fill: '#6366f1', strokeWidth: 1.5, stroke: '#fff' }}
                  activeDot={{ r: 5, strokeWidth: 0 }}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* 4. Sales Invoices Table */}
      <div className="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-100 dark:border-slate-700 overflow-hidden flex flex-col h-[500px]">
        <div className="p-6 flex flex-col sm:flex-row justify-between items-center gap-3 border-b border-slate-100 dark:border-slate-700 shrink-0">
          <h3 className="font-bold text-base text-slate-800 dark:text-white">Sales Invoices</h3>
          <div className="flex gap-2">
            <Button variant="outline" className="flex items-center text-xs h-8 px-3 border-slate-200 text-slate-600">
              <Filter size={12} className="mr-2" /> Filter
            </Button>
            <button className="text-slate-400 hover:text-slate-600 ml-1"><MoreHorizontal size={18} /></button>
          </div>
        </div>
        <div className="overflow-auto flex-1">
          <table className="w-full text-left border-collapse">
            <thead className="sticky top-0 bg-white dark:bg-slate-900 z-10 shadow-sm">
              <tr className="bg-slate-50 dark:bg-slate-900/50 text-slate-500 dark:text-slate-400 text-[10px] uppercase tracking-wider">
                <th className="px-6 py-4 font-semibold">Invoice No</th>
                <th className="px-6 py-4 font-semibold">Date</th>
                <th className="px-6 py-4 font-semibold">Customer</th>
                <th className="px-6 py-4 font-semibold text-center">Items</th>
                <th className="px-6 py-4 font-semibold text-right">Subtotal</th>
                <th className="px-6 py-4 font-semibold text-right">Discount</th>
                <th className="px-6 py-4 font-semibold text-right">Total</th>
                <th className="px-6 py-4 font-semibold text-right">Paid</th>
                <th className="px-6 py-4 font-semibold text-right">Balance</th>
                <th className="px-6 py-4 font-semibold text-right">Profit</th>
                <th className="px-6 py-4 font-semibold">Status</th>
                <th className="px-6 py-4 font-semibold text-center">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-700 text-sm">
              {sales.map((sale, index) => (
                <tr key={sale.saleId} className="hover:bg-slate-50 dark:hover:bg-slate-700/50 transition-colors">
                  <td className="px-6 py-4 text-slate-600 dark:text-slate-300 font-medium text-xs">{sale.invoiceNo || `#${1000 + index}`}</td>
                  <td className="px-6 py-4 text-slate-500 dark:text-slate-400 text-xs whitespace-nowrap">
                    {new Date(sale.saleDate).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })}
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-2">
                      <span className="font-medium text-slate-800 dark:text-slate-200 text-xs">
                        {sale.customer?.name || 'Walk-in Customer'}
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4 text-center text-slate-500 dark:text-slate-400 text-xs">
                    {sale.saleDetails?.length || 0}
                  </td>
                  <td className="px-6 py-4 text-right text-slate-500 dark:text-slate-400 text-xs">
                    {formatCurrency(sale.subTotal || 0)}
                  </td>
                  <td className="px-6 py-4 text-right text-red-500 text-xs">
                    {sale.discount ? `-${formatCurrency(sale.discount)}` : '-'}
                  </td>
                  <td className="px-6 py-4 text-right font-bold text-slate-800 dark:text-slate-200 text-xs">
                    {formatCurrency(sale.totalAmount)}
                  </td>
                  <td className="px-6 py-4 text-right text-emerald-600 text-xs font-medium">
                    {formatCurrency(sale.paidAmount || 0)}
                  </td>
                  <td className="px-6 py-4 text-right text-orange-600 text-xs font-medium">
                    {formatCurrency(sale.balanceAmount || 0)}
                  </td>
                  <td className="px-6 py-4 text-right text-indigo-600 text-xs font-medium">
                    {formatCurrency(sale.totalProfit || 0)}
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wide ${sale.isFullyPaid ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400' : 'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400'}`}>
                      {sale.isFullyPaid ? 'Paid' : 'Pending'}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-center">
                    {!sale.isFullyPaid && (
                      <button
                        onClick={() => {
                          setSelectedSale(sale);
                          setIsPaymentModalOpen(true);
                        }}
                        className="p-1.5 bg-indigo-50 dark:bg-indigo-900/30 text-indigo-600 dark:text-indigo-400 rounded-lg hover:bg-indigo-100 dark:hover:bg-indigo-900/50 transition-colors"
                        title="Add Payment"
                      >
                        <CreditCard size={14} />
                      </button>
                    )}
                  </td>
                </tr>
              ))}
              {(!sales || sales.length === 0) && (
                <tr>
                  <td colSpan={11} className="p-8 text-center text-slate-400">No sales found.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      <AddPaymentModal
        isOpen={isPaymentModalOpen}
        onClose={() => setIsPaymentModalOpen(false)}
        sale={selectedSale}
        onSuccess={(updatedSale) => {
          setSales(prev => prev.map(s => s.saleId === updatedSale.saleId ? updatedSale : s));
          fetchData(); // Also refresh charts/overall stats
        }}
      />
    </div>
  );
};

export default SmartOpsDashboard;
