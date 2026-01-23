import { useEffect, useState, useCallback } from "react";
import {
  Search,
  RefreshCw,
  List,
  Layers,
  ChevronLeft,
  ChevronRight,
  X,
  Package,
  TrendingUp,
  TrendingDown,
  Info,
  ArrowRightLeft,
  Filter,
  MoreVertical
} from "lucide-react";
import MainLayout from "@/components/layout/MainLayout.js";
import { useTheme } from "@/hooks/useTheme.js";
import {
  useStockSummaries,
  useStockTransactions,
  useStockBatches,
  useStocksLoading,
  useStocksError,
  useStockActions
} from "@/store/useStockStore.js";
import { StockSummaryDto, StockTransactionDto, FifoBatchDto } from "@/types/stock.js";

export default function StockPage() {
  const { isDark } = useTheme();

  const summaries = useStockSummaries();
  const isLoading = useStocksLoading();
  const error = useStocksError();
  const {
    fetchSummaries,
    fetchTransactions,
    fetchBatches,
    refreshSummary,
    clearError
  } = useStockActions();

  const [searchQuery, setSearchQuery] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;

  // Selected product state for modals
  const [selectedProduct, setSelectedProduct] = useState<StockSummaryDto | null>(null);
  const [activeModal, setActiveModal] = useState<"none" | "transactions" | "batches">("none");

  const transactions = useStockTransactions();
  const batches = useStockBatches();

  // Load summaries with debouncing
  useEffect(() => {
    const timer = setTimeout(() => {
      fetchSummaries(searchQuery);
    }, 500);
    return () => clearTimeout(timer);
  }, [searchQuery, fetchSummaries]);

  // Handle pagination
  const totalPages = Math.ceil(summaries.length / itemsPerPage) || 1;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentSummaries = summaries.slice(startIndex, startIndex + itemsPerPage);

  // Format helpers
  const formatCurrency = (val: number) =>
    new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(val);

  const formatNum = (val: number) =>
    new Intl.NumberFormat('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 2 }).format(val);

  const formatDate = (dateStr: string) => {
    if (!dateStr) return "-";
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const openTransactions = async (product: StockSummaryDto) => {
    setSelectedProduct(product);
    setActiveModal("transactions");
    await fetchTransactions(product.productId);
  };

  const openBatches = async (product: StockSummaryDto) => {
    setSelectedProduct(product);
    setActiveModal("batches");
    await fetchBatches(product.productId);
  };

  const closeModals = () => {
    setActiveModal("none");
    setSelectedProduct(null);
  };

  return (
    <MainLayout>
      <div className={`min-h-screen p-8 transition-colors duration-300 ${isDark ? 'bg-[#0f172a] text-white' : 'bg-gray-50 text-gray-900'}`}>
        {/* Header Section */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-10">
          <div>
            <div className="flex items-center gap-3 mb-2">
              <div className="p-2.5 bg-blue-600 rounded-2xl shadow-lg shadow-blue-600/20">
                <Package className="text-white" size={24} />
              </div>
              <h1 className="text-3xl font-black tracking-tight">Inventory & Stock</h1>
            </div>
            <p className={`${isDark ? 'text-gray-400' : 'text-gray-500'} font-medium`}>
              Monitor real-time stock levels, transaction history, and FIFO batches.
            </p>
          </div>

          <div className="flex items-center gap-4">
            <button
              onClick={() => fetchSummaries(searchQuery)}
              className={`flex items-center gap-2 px-5 py-3 rounded-2xl font-bold transition-all duration-300 ${isDark
                  ? 'bg-gray-800 hover:bg-gray-700 text-white'
                  : 'bg-white border border-gray-200 text-gray-700 hover:bg-gray-50 shadow-sm'
                }`}
            >
              <RefreshCw size={18} className={isLoading ? "animate-spin" : ""} />
              Sync Data
            </button>
          </div>
        </div>

        {/* Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
          {[
            {
              label: "Total Products",
              value: summaries.length,
              icon: Package,
              color: "text-blue-500",
              bg: "bg-blue-500/10"
            },
            {
              label: "Low Stock Items",
              value: summaries.filter(s => s.balance < 10).length,
              icon: TrendingDown,
              color: "text-amber-500",
              bg: "bg-amber-500/10"
            },
            {
              label: "Total Inventory Value",
              value: formatCurrency(summaries.reduce((acc, s) => acc + s.totalValue, 0)),
              icon: TrendingUp,
              color: "text-emerald-500",
              bg: "bg-emerald-500/10"
            }
          ].map((stat, i) => (
            <div key={i} className={`p-6 rounded-[2rem] border transition-all duration-300 hover:scale-[1.02] ${isDark ? 'bg-gray-900/50 border-gray-800' : 'bg-white border-gray-100 shadow-xl shadow-gray-200/20'
              }`}>
              <div className="flex items-center justify-between mb-4">
                <div className={`p-4 rounded-2xl ${stat.bg} ${stat.color}`}>
                  <stat.icon size={24} />
                </div>
                <div className={`text-xs font-black uppercase tracking-widest ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>Current Live</div>
              </div>
              <div className="text-3xl font-black mb-1">{stat.value}</div>
              <div className={`text-sm font-bold ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>{stat.label}</div>
            </div>
          ))}
        </div>

        {/* Main Content Card */}
        <div className={`rounded-[2.5rem] border overflow-hidden ${isDark ? 'bg-gray-900/50 border-gray-800' : 'bg-white border-gray-100 shadow-2xl shadow-gray-200/40'
          }`}>
          {/* Controls Bar */}
          <div className={`p-8 border-b flex flex-col sm:flex-row items-center justify-between gap-6 ${isDark ? 'border-gray-800' : 'border-gray-50'}`}>
            <div className="relative w-full sm:w-96 group">
              <Search className={`absolute left-5 top-1/2 -translate-y-1/2 transition-colors duration-300 ${isDark ? 'text-gray-600 group-focus-within:text-blue-400' : 'text-gray-400 group-focus-within:text-blue-500'}`} size={20} />
              <input
                type="text"
                placeholder="Find product by name or ID..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={`w-full pl-14 pr-6 py-4 rounded-2xl font-semibold transition-all duration-300 focus:outline-none focus:ring-4 ${isDark
                    ? 'bg-gray-800/50 border-transparent focus:bg-gray-800 focus:ring-blue-500/20 text-white placeholder-gray-600'
                    : 'bg-gray-50 border-transparent focus:bg-white focus:ring-blue-500/10 text-gray-900 placeholder-gray-400 shadow-inner'
                  }`}
              />
            </div>

            <div className="flex items-center gap-3">
              <button className={`p-3 rounded-xl transition-colors ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white' : 'bg-gray-50 text-gray-500 hover:text-gray-900'}`}>
                <Filter size={20} />
              </button>
              <button className={`p-3 rounded-xl transition-colors ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white' : 'bg-gray-50 text-gray-500 hover:text-gray-900'}`}>
                <MoreVertical size={20} />
              </button>
            </div>
          </div>

          {/* Table Container */}
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className={`${isDark ? 'bg-gray-800/20' : 'bg-gray-50/50'}`}>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400">Product Details</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">In / Out</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">Available Stock</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-right">Valuation (Avg)</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-right">Total Value</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">Actions</th>
                </tr>
              </thead>
              <tbody className={`divide-y ${isDark ? 'divide-gray-800' : 'divide-gray-50'}`}>
                {isLoading && summaries.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-8 py-20 text-center">
                      <div className="flex flex-col items-center gap-4">
                        <div className="h-12 w-12 border-4 border-blue-600/30 border-t-blue-600 rounded-full animate-spin" />
                        <p className={`text-lg font-bold ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>Updating inventory records...</p>
                      </div>
                    </td>
                  </tr>
                ) : summaries.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-8 py-20 text-center">
                      <div className="flex flex-col items-center gap-4 text-gray-400">
                        <div className="p-8 bg-gray-500/5 rounded-full">
                          <Package size={64} opacity={0.3} />
                        </div>
                        <p className="text-xl font-black">No stock items found</p>
                        <p className="text-sm font-medium">Try a different search term or check back later.</p>
                      </div>
                    </td>
                  </tr>
                ) : (
                  currentSummaries.map((s) => {
                    const isLowStock = s.balance < 10;
                    return (
                      <tr key={s.stockSummaryId} className={`group transition-all duration-300 ${isDark ? 'hover:bg-gray-800/30' : 'hover:bg-blue-50/30'}`}>
                        <td className="px-8 py-6">
                          <div className="flex items-center gap-5">
                            <div className={`h-14 w-14 rounded-2xl flex items-center justify-center font-black text-xl select-none transition-transform group-hover:scale-110 ${isDark ? 'bg-gray-800 text-blue-400 border border-gray-700' : 'bg-blue-50 text-blue-600 border border-blue-100'
                              }`}>
                              {s.productName.charAt(0).toUpperCase()}
                            </div>
                            <div>
                              <div className={`text-lg font-black ${isDark ? 'text-white' : 'text-gray-900'}`}>{s.productName}</div>
                              <div className={`text-xs mt-1 font-bold tracking-widest uppercase ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>ID: P#{s.productId}</div>
                            </div>
                          </div>
                        </td>
                        <td className="px-8 py-6">
                          <div className="flex flex-col items-center gap-1.5">
                            <div className="flex items-center gap-2 text-xs font-black">
                              <div className="w-8 flex justify-end text-emerald-500">+{formatNum(s.totalIn)}</div>
                              <div className="w-2 h-0.5 bg-gray-300 rounded-full" />
                              <div className="w-8 text-rose-500">-{formatNum(s.totalOut)}</div>
                            </div>
                            <div className={`text-[10px] font-black uppercase tracking-tighter ${isDark ? 'text-gray-600' : 'text-gray-400'}`}>Flow Vol. {formatNum(s.totalIn + s.totalOut)}</div>
                          </div>
                        </td>
                        <td className="px-8 py-6">
                          <div className="flex flex-col items-center">
                            <div className={`px-5 py-2 rounded-2xl font-black text-lg transition-all shadow-sm ${isLowStock
                                ? 'bg-amber-500/10 text-amber-500 border border-amber-500/20'
                                : 'bg-emerald-500/10 text-emerald-500 border border-emerald-500/20'
                              }`}>
                              {formatNum(s.balance)}
                            </div>
                            {isLowStock && (
                              <div className="mt-1.5 text-[10px] font-black text-amber-500 uppercase flex items-center gap-1">
                                <TrendingDown size={10} /> REORDER SOON
                              </div>
                            )}
                          </div>
                        </td>
                        <td className="px-8 py-6 text-right">
                          <div className={`font-black ${isDark ? 'text-gray-300' : 'text-gray-700'}`}>{formatCurrency(s.averageCost)}</div>
                          <div className={`text-[10px] font-black uppercase ${isDark ? 'text-gray-600' : 'text-gray-400'}`}>Unit WAC</div>
                        </td>
                        <td className="px-8 py-6 text-right font-black">
                          <div className={`text-xl ${isDark ? 'text-white' : 'text-gray-900'}`}>{formatCurrency(s.totalValue)}</div>
                          <div className={`text-[10px] font-black uppercase ${isDark ? 'text-gray-600' : 'text-gray-400'}`}>Last Sync: {new Date(s.lastUpdated).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</div>
                        </td>
                        <td className="px-8 py-6">
                          <div className="flex items-center justify-center gap-2.5">
                            <button
                              onClick={() => openTransactions(s)}
                              className={`p-3 rounded-2xl transition-all duration-200 group/btn ${isDark ? 'bg-gray-800 text-blue-400 hover:bg-blue-600 hover:text-white' : 'bg-blue-50 text-blue-600 hover:bg-blue-600 hover:text-white'}`}
                              title="Movement History"
                            >
                              <ArrowRightLeft size={18} />
                            </button>
                            <button
                              onClick={() => openBatches(s)}
                              className={`p-3 rounded-2xl transition-all duration-200 group/btn ${isDark ? 'bg-gray-800 text-amber-400 hover:bg-amber-600 hover:text-white' : 'bg-amber-50 text-amber-600 hover:bg-amber-600 hover:text-white'}`}
                              title="FIFO Batches"
                            >
                              <Layers size={18} />
                            </button>
                            <button
                              onClick={() => refreshSummary(s.productId)}
                              className={`p-3 rounded-2xl transition-all duration-200 group/btn ${isDark ? 'bg-gray-800 text-emerald-400 hover:bg-emerald-600 hover:text-white' : 'bg-emerald-50 text-emerald-600 hover:bg-emerald-600 hover:text-white'}`}
                              title="Recalculate Table"
                            >
                              <RefreshCw size={18} />
                            </button>
                          </div>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination Section */}
          {totalPages > 1 && (
            <div className={`px-8 py-8 border-t flex flex-col sm:flex-row items-center justify-between gap-6 ${isDark ? 'border-gray-800 bg-gray-900/20' : 'border-gray-50 bg-gray-50/20'}`}>
              <div className={`text-sm font-bold ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>
                Showing <span className={isDark ? 'text-white' : 'text-gray-900'}>{startIndex + 1}</span> to <span className={isDark ? 'text-white' : 'text-gray-900'}>{Math.min(startIndex + itemsPerPage, summaries.length)}</span> of <span className={isDark ? 'text-white' : 'text-gray-900'}>{summaries.length}</span> Stock Records
              </div>

              <div className="flex items-center gap-2.5">
                <button
                  onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                  className={`p-3 rounded-2xl transition-all duration-300 disabled:opacity-20 disabled:grayscale ${isDark ? 'bg-gray-800 text-white hover:bg-gray-700' : 'bg-white border text-gray-700 hover:bg-gray-50 shadow-sm'
                    }`}
                >
                  <ChevronLeft size={20} />
                </button>

                <div className="flex items-center gap-1.5 px-3">
                  {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                    <button
                      key={page}
                      onClick={() => setCurrentPage(page)}
                      className={`w-11 h-11 rounded-2xl font-black text-sm transition-all duration-300 ${currentPage === page
                          ? 'bg-blue-600 text-white shadow-xl shadow-blue-600/30 scale-110 ring-4 ring-blue-600/10'
                          : isDark ? 'bg-gray-800 text-gray-500 hover:bg-gray-750 hover:text-white' : 'bg-white border text-gray-400 hover:bg-gray-50 shadow-sm'
                        }`}
                    >
                      {page}
                    </button>
                  ))}
                </div>

                <button
                  onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                  className={`p-3 rounded-2xl transition-all duration-300 disabled:opacity-20 disabled:grayscale ${isDark ? 'bg-gray-800 text-white hover:bg-gray-700' : 'bg-white border text-gray-700 hover:bg-gray-50 shadow-sm'
                    }`}
                >
                  <ChevronRight size={20} />
                </button>
              </div>
            </div>
          )}
        </div>

        {/* MODALS */}

        {/* Transactions Modal */}
        {activeModal === "transactions" && selectedProduct && (
          <div className="fixed inset-0 z-[100] flex items-center justify-center p-6">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={closeModals} />
            <div className={`relative w-full max-w-5xl max-h-[90vh] rounded-[2.5rem] shadow-2xl border flex flex-col animate-in fade-in zoom-in duration-300 ${isDark ? 'bg-gray-900 border-gray-800' : 'bg-white border-gray-100'
              }`}>
              <div className="p-8 border-b flex items-center justify-between">
                <div className="flex items-center gap-4">
                  <div className="p-4 bg-blue-500/10 text-blue-500 rounded-3xl">
                    <ArrowRightLeft size={28} />
                  </div>
                  <div>
                    <h2 className="text-2xl font-black">{selectedProduct.productName}</h2>
                    <p className="text-gray-500 font-bold uppercase tracking-widest text-xs">Movement Transaction History</p>
                  </div>
                </div>
                <button onClick={closeModals} className="p-4 rounded-3xl hover:bg-red-500/10 hover:text-red-500 transition-colors">
                  <X size={24} />
                </button>
              </div>

              <div className="flex-1 overflow-y-auto p-8">
                {isLoading ? (
                  <div className="flex flex-col items-center justify-center py-20 gap-4">
                    <div className="w-12 h-12 border-4 border-blue-600/20 border-t-blue-600 rounded-full animate-spin" />
                    <p className="font-bold text-gray-500">Retrieving movements...</p>
                  </div>
                ) : transactions.length === 0 ? (
                  <div className="text-center py-20">
                    <p className="text-gray-400 font-bold">No movements recorded for this product.</p>
                  </div>
                ) : (
                  <div className="grid grid-cols-1 gap-4">
                    {transactions.map((t) => (
                      <div key={t.stockTransactionId} className={`p-6 rounded-[2rem] border flex items-center justify-between transition-colors ${isDark ? 'bg-gray-800/40 border-gray-800' : 'bg-gray-50 border-gray-100'
                        }`}>
                        <div className="flex items-center gap-6">
                          <div className={`p-4 rounded-2xl ${t.transactionType === 'S' || t.quantity < 0 ? 'bg-rose-500/10 text-rose-500' : 'bg-emerald-500/10 text-emerald-500'
                            }`}>
                            {t.transactionType === 'S' || t.quantity < 0 ? <TrendingDown size={24} /> : <TrendingUp size={24} />}
                          </div>
                          <div>
                            <div className="flex items-center gap-3 mb-1">
                              <div className="font-black text-lg">{t.transactionTypeName}</div>
                              <div className="text-[10px] font-black uppercase tracking-widest px-2 py-0.5 rounded-full bg-gray-500/10 text-gray-500 border border-gray-500/10">Ref {t.referenceId}</div>
                            </div>
                            <div className="text-sm font-bold text-gray-500">{formatDate(t.createdAt)}</div>
                          </div>
                        </div>
                        <div className="flex items-center gap-10">
                          <div className="text-right">
                            <div className={`text-xl font-black ${t.quantity < 0 ? 'text-rose-500' : 'text-emerald-500'}`}>
                              {t.quantity < 0 ? '-' : '+'}{formatNum(Math.abs(t.quantity))}
                            </div>
                            <div className="text-[10px] font-black uppercase text-gray-400 tracking-tighter">Quantity</div>
                          </div>
                          <div className="text-right">
                            <div className="text-xl font-black">{formatCurrency(t.unitCost)}</div>
                            <div className="text-[10px] font-black uppercase text-gray-400 tracking-tighter">Market Cost</div>
                          </div>
                          <div className="text-right min-w-[120px]">
                            <div className="text-xl font-black">{formatCurrency(t.totalCost)}</div>
                            <div className="text-[10px] font-black uppercase text-gray-400 tracking-tighter">Total Flow</div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div className="p-8 border-t flex justify-end">
                <button onClick={closeModals} className={`px-10 py-4 rounded-2xl font-black ${isDark ? 'bg-gray-800 hover:bg-gray-750' : 'bg-gray-100 hover:bg-gray-200'}`}>Close Window</button>
              </div>
            </div>
          </div>
        )}

        {/* Batches Modal */}
        {activeModal === "batches" && selectedProduct && (
          <div className="fixed inset-0 z-[100] flex items-center justify-center p-6">
            <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={closeModals} />
            <div className={`relative w-full max-w-5xl max-h-[90vh] rounded-[2.5rem] shadow-2xl border flex flex-col animate-in fade-in zoom-in duration-300 ${isDark ? 'bg-gray-900 border-gray-800' : 'bg-white border-gray-100'
              }`}>
              <div className="p-8 border-b flex items-center justify-between">
                <div className="flex items-center gap-4">
                  <div className="p-4 bg-amber-500/10 text-amber-500 rounded-3xl">
                    <Layers size={28} />
                  </div>
                  <div>
                    <h2 className="text-2xl font-black">{selectedProduct.productName}</h2>
                    <p className="text-gray-500 font-bold uppercase tracking-widest text-xs">Active FIFO Stock Layers</p>
                  </div>
                </div>
                <button onClick={closeModals} className="p-4 rounded-3xl hover:bg-red-500/10 hover:text-red-500 transition-colors">
                  <X size={24} />
                </button>
              </div>

              <div className="flex-1 overflow-y-auto p-8">
                {isLoading ? (
                  <div className="flex flex-col items-center justify-center py-20 gap-4">
                    <div className="w-12 h-12 border-4 border-amber-600/20 border-t-amber-600 rounded-full animate-spin" />
                    <p className="font-bold text-gray-500">Analyzing batch data...</p>
                  </div>
                ) : batches.length === 0 ? (
                  <div className="text-center py-20">
                    <p className="text-gray-400 font-bold">No active batches available for this product.</p>
                  </div>
                ) : (
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    {batches.map((b) => (
                      <div key={b.purchaseDetailId} className={`p-6 rounded-[2.5rem] border relative overflow-hidden group transition-all duration-300 hover:shadow-xl ${isDark ? 'bg-gray-800/40 border-gray-800 hover:bg-gray-800' : 'bg-gray-50 border-gray-100 hover:bg-white'
                        }`}>
                        <div className="flex justify-between items-start mb-6">
                          <div>
                            <div className="text-2xl font-black mb-1">{b.batchNo}</div>
                            <div className={`text-xs font-black uppercase tracking-widest ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>Purchased {formatDate(b.purchaseDate)}</div>
                          </div>
                          <div className="px-4 py-2 bg-blue-500 text-white rounded-2xl text-xs font-black">ACTIVE</div>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                          <div className={`p-4 rounded-2xl ${isDark ? 'bg-gray-900/50' : 'bg-white shadow-sm'}`}>
                            <div className="text-xs font-bold text-gray-500 uppercase mb-1">Remaining</div>
                            <div className="text-xl font-black text-blue-500">{formatNum(b.availableQuantity)} Units</div>
                          </div>
                          <div className={`p-4 rounded-2xl ${isDark ? 'bg-gray-900/50' : 'bg-white shadow-sm'}`}>
                            <div className="text-xs font-bold text-gray-500 uppercase mb-1">Unit Cost</div>
                            <div className="text-xl font-black">{formatCurrency(b.unitCost)}</div>
                          </div>
                        </div>

                        {b.expiryDate && (
                          <div className={`mt-4 flex items-center gap-2 p-3 rounded-xl border ${new Date(b.expiryDate) < new Date() ? 'bg-red-500/10 border-red-500/20 text-red-500' : 'bg-gray-500/5 border-gray-500/10 text-gray-500'
                            }`}>
                            <Info size={14} />
                            <div className="text-[10px] font-black uppercase tracking-widest">Expires: {formatDate(b.expiryDate)}</div>
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div className="p-8 border-t flex justify-end">
                <button onClick={closeModals} className={`px-10 py-4 rounded-2xl font-black ${isDark ? 'bg-gray-800 hover:bg-gray-750' : 'bg-gray-100 hover:bg-gray-200'}`}>Close Window</button>
              </div>
            </div>
          </div>
        )}

      </div>
    </MainLayout>
  );
}
