import { useState, useEffect, useCallback } from 'react';
import {
  Plus,
  Search,
  Eye,
  Truck,
  X,
  ChevronLeft,
  ChevronRight,
  Package,
  Calendar,
  Filter,
  ArrowUpRight,
  TrendingUp,
  Clock,
  CheckCircle2,
  AlertCircle,
  MoreVertical,
  Printer,
  Download,
  Trash2
} from 'lucide-react';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import {
  usePurchases,
  usePurchasesLoading,
  usePurchasesError,
  usePurchaseActions,
  useSelectedPurchase
} from '@/store/usePurchaseStore.js';
import { useSuppliers, useSupplierActions } from '@/store/useSupplierStore.js';
import { useProducts, useProductActions } from '@/store/useProductStore.js';
import { PurchaseDTO } from '@/types/purchase.js';
import PurchaseModal from '@/components/purchase/PurchaseModal.js';

export default function PurchasePage() {
  const { isDark } = useTheme();
  const purchases = usePurchases();
  const isLoading = usePurchasesLoading();
  const error = usePurchasesError();
  const selectedPurchase = useSelectedPurchase();
  const {
    fetchPurchases,
    receivePurchase,
    cancelPurchase,
    selectPurchase,
    clearError,
    getNextPurchaseNumber
  } = usePurchaseActions();

  const suppliers = useSuppliers();
  const { fetchSuppliers } = useSupplierActions();
  const products = useProducts();
  const { fetchProducts } = useProductActions();

  // Filters state
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Stats calculation
  const stats = {
    total: purchases.length,
    pending: purchases.filter(p => p.status === 'Pending').length,
    received: purchases.filter(p => p.status === 'Received').length,
    totalValue: purchases.reduce((acc, p) => acc + p.totalAmount, 0)
  };

  // Initial load
  useEffect(() => {
    fetchPurchases();
    fetchSuppliers();
    fetchProducts();
  }, []);

  // Format Helpers
  const formatCurrency = (val: number) =>
    new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(val);

  const formatDate = (dateStr: string) => {
    if (!dateStr) return "-";
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getStatusStyle = (status: string) => {
    switch (status) {
      case 'Received': return 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20';
      case 'Pending': return 'bg-amber-500/10 text-amber-500 border-amber-500/20';
      case 'Cancelled': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      default: return 'bg-gray-500/10 text-gray-500 border-gray-500/20';
    }
  };

  return (
    <MainLayout>
      <div className={`min-h-screen p-8 transition-colors duration-300 ${isDark ? 'bg-[#0f172a] text-white' : 'bg-gray-50 text-gray-900'}`}>

        {/* Header Section */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-10">
          <div>
            <div className="flex items-center gap-3 mb-2">
              <div className="p-2.5 bg-indigo-600 rounded-2xl shadow-lg shadow-indigo-600/20">
                <Truck className="text-white" size={24} />
              </div>
              <h1 className="text-3xl font-black tracking-tight">Purchase Orders</h1>
            </div>
            <p className={`${isDark ? 'text-gray-400' : 'text-gray-500'} font-medium`}>
              Inbound logistics, supplier invoices, and stock replenishment.
            </p>
          </div>

          <div className="flex items-center gap-4">
            <button
              onClick={() => setIsModalOpen(true)}
              className="flex items-center gap-2 px-6 py-3.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-2xl font-bold shadow-lg shadow-indigo-600/25 transition-all hover:scale-[1.02] active:scale-[0.98]"
            >
              <Plus size={20} className="stroke-[3]" />
              New Purchase
            </button>
          </div>
        </div>

        {/* Dynamic Stats Grid */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-10">
          {[
            { label: "Total Orders", value: stats.total, icon: Truck, color: "text-blue-500", bg: "bg-blue-500/10" },
            { label: "Pending Receipt", value: stats.pending, icon: Clock, color: "text-amber-500", bg: "bg-amber-500/10" },
            { label: "Successfully Received", value: stats.received, icon: CheckCircle2, color: "text-emerald-500", bg: "bg-emerald-500/10" },
            { label: "Total Capital Spent", value: formatCurrency(stats.totalValue), icon: TrendingUp, color: "text-indigo-500", bg: "bg-indigo-500/10" }
          ].map((stat, i) => (
            <div key={i} className={`p-6 rounded-[2rem] border transition-all duration-300 hover:scale-[1.02] ${isDark ? 'bg-gray-900/50 border-gray-800' : 'bg-white border-gray-100 shadow-xl shadow-gray-200/20'
              }`}>
              <div className="flex items-center justify-between mb-4">
                <div className={`p-4 rounded-2xl ${stat.bg} ${stat.color}`}>
                  <stat.icon size={24} />
                </div>
                <ArrowUpRight size={20} className="text-gray-400 opacity-50" />
              </div>
              <div className="text-2xl font-black mb-1">{stat.value}</div>
              <div className={`text-xs font-bold uppercase tracking-widest ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>{stat.label}</div>
            </div>
          ))}
        </div>

        {/* Global Toolbar */}
        <div className={`rounded-[2.5rem] border overflow-hidden ${isDark ? 'bg-gray-900/50 border-gray-800' : 'bg-white border-gray-100 shadow-2xl shadow-gray-200/40'
          }`}>
          <div className={`p-8 border-b flex flex-col sm:flex-row items-center justify-between gap-6 ${isDark ? 'border-gray-800' : 'border-gray-50'}`}>
            <div className="relative w-full sm:w-96 group">
              <Search className={`absolute left-5 top-1/2 -translate-y-1/2 transition-colors duration-300 ${isDark ? 'text-gray-600 group-focus-within:text-indigo-400' : 'text-gray-400 group-focus-within:text-indigo-500'}`} size={20} />
              <input
                type="text"
                placeholder="Search by invoice or supplier..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={`w-full pl-14 pr-6 py-4 rounded-2xl font-semibold transition-all duration-300 focus:outline-none focus:ring-4 ${isDark
                  ? 'bg-gray-800/50 border-transparent focus:bg-gray-800 focus:ring-indigo-500/20 text-white placeholder-gray-600'
                  : 'bg-gray-50 border-transparent focus:bg-white focus:ring-indigo-500/10 text-gray-900 placeholder-gray-400 shadow-inner'
                  }`}
              />
            </div>

            <div className="flex items-center gap-3">
              <button className={`p-3 rounded-xl transition-colors ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white' : 'bg-gray-50 text-gray-500 hover:text-gray-900'}`}>
                <Calendar size={20} />
              </button>
              <button className={`p-3 rounded-xl transition-colors ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white' : 'bg-gray-50 text-gray-500 hover:text-gray-900'}`}>
                <Filter size={20} />
              </button>
            </div>
          </div>

          {/* Premium Table Content */}
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className={`${isDark ? 'bg-gray-800/20' : 'bg-gray-50/50'}`}>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400">Invoice Details</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400">Supplier</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400">Items</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-right">Total Amount</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">Status</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">Actions</th>
                </tr>
              </thead>
              <tbody className={`divide-y ${isDark ? 'divide-gray-800' : 'divide-gray-50'}`}>
                {isLoading ? (
                  <tr>
                    <td colSpan={6} className="px-8 py-20 text-center">
                      <div className="flex flex-col items-center gap-4">
                        <div className="h-10 w-10 border-4 border-indigo-600/30 border-t-indigo-600 rounded-full animate-spin" />
                        <p className="font-bold text-gray-500">Loading purchase records...</p>
                      </div>
                    </td>
                  </tr>
                ) : purchases.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-8 py-20 text-center">
                      <div className="flex flex-col items-center gap-4 opacity-40">
                        <Truck size={64} className="text-gray-400" />
                        <p className="text-xl font-black">No purchases found</p>
                      </div>
                    </td>
                  </tr>
                ) : (
                  purchases.map((p) => (
                    <tr key={p.purchaseId} className={`group transition-all duration-300 ${isDark ? 'hover:bg-gray-800/30' : 'hover:bg-indigo-50/30'}`}>
                      <td className="px-8 py-6">
                        <div className="flex items-center gap-4">
                          <div className={`h-11 w-11 rounded-xl flex items-center justify-center ${isDark ? 'bg-gray-800' : 'bg-gray-100'}`}>
                            <Package size={20} className="text-indigo-500" />
                          </div>
                          <div>
                            <div className="font-black text-lg">{p.purchaseNumber}</div>
                            <div className="text-xs font-bold text-gray-400 uppercase">{formatDate(p.purchaseDate)}</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-8 py-6">
                        <div className="font-bold">{p.supplierName}</div>
                        <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Global Supplier</div>
                      </td>
                      <td className="px-8 py-6">
                        <div className="flex items-center gap-2">
                          <span className="font-black text-lg">{p.purchaseDetails.length}</span>
                          <span className="text-xs font-bold text-gray-400 uppercase">Lines</span>
                        </div>
                      </td>
                      <td className="px-8 py-6 text-right">
                        <div className="font-black text-lg text-indigo-500">{formatCurrency(p.totalAmount)}</div>
                        <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Gross Total</div>
                      </td>
                      <td className="px-8 py-6 text-center">
                        <span className={`px-4 py-1.5 rounded-xl border text-[10px] font-black uppercase tracking-widest ${getStatusStyle(p.status)}`}>
                          {p.status}
                        </span>
                      </td>
                      <td className="px-8 py-6">
                        <div className="flex items-center justify-center gap-2">
                          <button
                            onClick={() => selectPurchase(p)}
                            className={`p-3 rounded-2xl transition-all ${isDark ? 'bg-gray-800 hover:bg-indigo-600' : 'bg-gray-100 hover:bg-indigo-600'} hover:text-white`}
                          >
                            <Eye size={18} />
                          </button>
                          {p.status === 'Pending' && (
                            <>
                              <button
                                onClick={() => receivePurchase(p.purchaseId)}
                                className={`p-3 rounded-2xl transition-all ${isDark ? 'bg-gray-800 hover:bg-emerald-600' : 'bg-gray-100 hover:bg-emerald-600'} hover:text-white`}
                              >
                                <CheckCircle2 size={18} />
                              </button>
                              <button
                                onClick={() => cancelPurchase(p.purchaseId)}
                                className={`p-3 rounded-2xl transition-all ${isDark ? 'bg-gray-800 hover:bg-rose-600' : 'bg-gray-100 hover:bg-rose-600'} hover:text-white text-rose-500`}
                              >
                                <X size={18} />
                              </button>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Purchase Details Sidebar/Modal */}
        {selectedPurchase && (
          <div className="fixed inset-0 z-[100] flex items-center justify-center p-6">
            <div className="absolute inset-0 bg-black/60 backdrop-blur-md" onClick={() => selectPurchase(null)} />
            <div className={`relative w-full max-w-4xl max-h-[90vh] rounded-[3rem] shadow-2xl overflow-hidden flex flex-col animate-in fade-in zoom-in duration-300 ${isDark ? 'bg-[#0f172a] border border-gray-800' : 'bg-white border border-gray-100'
              }`}>
              {/* Header */}
              <div className="p-10 border-b flex items-center justify-between bg-indigo-600">
                <div className="flex items-center gap-6">
                  <div className="h-20 w-20 bg-white/20 backdrop-blur-xl rounded-3xl flex items-center justify-center text-white">
                    <Truck size={36} />
                  </div>
                  <div>
                    <h2 className="text-3xl font-black text-white">{selectedPurchase.purchaseNumber}</h2>
                    <div className="flex items-center gap-3 mt-1.5">
                      <span className="text-white/70 font-bold uppercase tracking-widest text-xs">Supplier: {selectedPurchase.supplierName}</span>
                      <div className="w-1 h-1 bg-white/30 rounded-full" />
                      <span className="text-white/70 font-bold uppercase tracking-widest text-xs">{selectedPurchase.status}</span>
                    </div>
                  </div>
                </div>
                <button onClick={() => selectPurchase(null)} className="p-4 bg-white/10 hover:bg-white/20 text-white rounded-3xl transition-colors">
                  <X size={24} />
                </button>
              </div>

              {/* Content */}
              <div className="flex-1 p-10 overflow-y-auto space-y-10">
                <div className="grid grid-cols-2 gap-8">
                  <div className={`p-8 rounded-[2rem] ${isDark ? 'bg-gray-800/30' : 'bg-gray-50'}`}>
                    <h4 className="text-xs font-black text-gray-400 uppercase tracking-[0.2em] mb-4">Financial Overview</h4>
                    <div className="flex items-end justify-between">
                      <div className="text-4xl font-black text-indigo-500">{formatCurrency(selectedPurchase.totalAmount)}</div>
                      <div className="text-xs font-bold text-gray-500 mb-1">Total Gross</div>
                    </div>
                  </div>
                  <div className={`p-8 rounded-[2rem] ${isDark ? 'bg-gray-800/30' : 'bg-gray-50'}`}>
                    <h4 className="text-xs font-black text-gray-400 uppercase tracking-[0.2em] mb-4">Activity Timeline</h4>
                    <div className="space-y-3">
                      <div className="flex items-center gap-3">
                        <Clock size={16} className="text-indigo-500" />
                        <span className="text-sm font-bold">Created {formatDate(selectedPurchase.createdAt)}</span>
                      </div>
                      {selectedPurchase.receivedDate && (
                        <div className="flex items-center gap-3">
                          <CheckCircle2 size={16} className="text-emerald-500" />
                          <span className="text-sm font-bold">Received {formatDate(selectedPurchase.receivedDate)}</span>
                        </div>
                      )}
                    </div>
                  </div>
                </div>

                <div>
                  <h4 className="text-xs font-black text-gray-400 uppercase tracking-[0.2em] mb-6">Manifest Items</h4>
                  <div className="space-y-3">
                    {selectedPurchase.purchaseDetails.map((item, idx) => (
                      <div key={idx} className={`p-6 rounded-3xl border flex items-center justify-between ${isDark ? 'bg-gray-800/20 border-gray-800' : 'bg-white border-gray-100'
                        }`}>
                        <div className="flex items-center gap-5">
                          <div className={`h-12 w-12 rounded-2xl flex items-center justify-center font-black ${isDark ? 'bg-gray-800' : 'bg-gray-100 text-indigo-600'}`}>
                            {idx + 1}
                          </div>
                          <div>
                            <div className="font-black text-lg">{item.productName}</div>
                            <div className="text-[10px] font-black text-gray-400 uppercase">Batch: {item.batchNo || 'N/A'}</div>
                          </div>
                        </div>
                        <div className="flex items-center gap-12 text-right">
                          <div>
                            <div className="text-lg font-black">{item.quantity}</div>
                            <div className="text-[10px] font-black text-gray-400 uppercase">Quantity</div>
                          </div>
                          <div className="min-w-[100px]">
                            <div className="text-lg font-black text-indigo-500">{formatCurrency(item.totalCost)}</div>
                            <div className="text-[10px] font-black text-gray-400 uppercase">Line Total</div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                {selectedPurchase.notes && (
                  <div className={`p-8 rounded-[2rem] border-l-4 border-indigo-600 ${isDark ? 'bg-indigo-500/5' : 'bg-indigo-50'}`}>
                    <h4 className="text-xs font-black text-indigo-600 uppercase tracking-widest mb-3">Auditor Notes</h4>
                    <p className="text-sm font-medium leading-relaxed opacity-80">{selectedPurchase.notes}</p>
                  </div>
                )}
              </div>

              {/* Footer Actions */}
              <div className="p-10 border-t flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <button className={`flex items-center gap-2 px-6 py-3 rounded-2xl font-bold transition-all ${isDark ? 'bg-gray-800 hover:bg-gray-700' : 'bg-gray-100 hover:bg-gray-200'}`}>
                    <Printer size={18} />
                    Print Invoice
                  </button>
                  <button className={`flex items-center gap-2 px-6 py-3 rounded-2xl font-bold transition-all ${isDark ? 'bg-gray-800 hover:bg-gray-700' : 'bg-gray-100 hover:bg-gray-200'}`}>
                    <Download size={18} />
                    Export PDF
                  </button>
                </div>
                <button
                  onClick={() => selectPurchase(null)}
                  className="px-10 py-3 bg-indigo-600 text-white rounded-2xl font-black shadow-lg shadow-indigo-600/20 hover:scale-[1.02] active:scale-[0.98] transition-all"
                >
                  Done
                </button>
              </div>
            </div>
          </div>
        )}

        <PurchaseModal
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
        />

      </div>
    </MainLayout>
  );
}