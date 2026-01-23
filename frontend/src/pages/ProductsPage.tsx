import { useState, useEffect } from 'react';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  Package,
  X,
  LayoutGrid,
  Filter,
  ArrowUpRight,
  TrendingUp,
  AlertCircle,
  Eye,
  Settings2,
  Tag,
  Box,
  CheckCircle2,
  Clock
} from 'lucide-react';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import {
  useProducts,
  useProductsLoading,
  useProductsError,
  useProductActions,
  useSelectedProduct
} from '@/store/useProductStore.js';
import { useCategoryActions } from '@/store/useCategoryStore.js';
import ProductModal from '@/components/product/ProductModal.js';
import { ProductDTO } from '@/types/product.js';

export default function ProductsPage() {
  const { isDark } = useTheme();
  const products = useProducts();
  const isLoading = useProductsLoading();
  const error = useProductsError();
  const selectedProduct = useSelectedProduct();
  const {
    fetchProducts,
    deleteProduct,
    selectProduct,
    clearError
  } = useProductActions();
  const { fetchCategories } = useCategoryActions();

  // Navigation and Filtering
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Stats calculation
  const stats = {
    total: products.length,
    active: products.filter(p => p.isActive).length,
    critical: products.filter(p => p.isActive && (p.stockBalance || 0) <= (p.reorderLevel || 0)).length,
    totalValue: products.reduce((acc, p) => acc + (p.totalValue || 0), 0)
  };

  // Initial Load
  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  const formatCurrency = (val: number) =>
    new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(val);

  const handleEdit = (product: ProductDTO) => {
    selectProduct(product);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    if (window.confirm("Are you sure you want to deactivate this product? It will be archived from the active catalog.")) {
      await deleteProduct(id);
    }
  };

  return (
    <MainLayout>
      <div className={`min-h-screen p-8 transition-colors duration-300 ${isDark ? 'bg-[#0f172a] text-white' : 'bg-gray-50 text-gray-900'}`}>

        {/* Futuristic Header */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-10">
          <div>
            <div className="flex items-center gap-3 mb-2">
              <div className="p-2.5 bg-indigo-600 rounded-2xl shadow-lg shadow-indigo-600/20">
                <Box className="text-white" size={24} />
              </div>
              <h1 className="text-3xl font-black tracking-tight">Product Catalog</h1>
            </div>
            <p className={`${isDark ? 'text-gray-400' : 'text-gray-500'} font-medium`}>
              Master inventory registry, SKU management, and reorder controls.
            </p>
          </div>

          <div className="flex items-center gap-4">
            <button
              onClick={() => { selectProduct(null); setIsModalOpen(true); }}
              className="flex items-center gap-2 px-6 py-3.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-2xl font-bold shadow-lg shadow-indigo-600/25 transition-all hover:scale-[1.02] active:scale-[0.98]"
            >
              <Plus size={20} className="stroke-[3]" />
              Add Entry
            </button>
          </div>
        </div>

        {/* Global Analytics Overview */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-10">
          {[
            { label: "Inventory Index", value: stats.total, icon: Box, color: "text-blue-500", bg: "bg-blue-500/10" },
            { label: "Active Tradables", value: stats.active, icon: CheckCircle2, color: "text-emerald-500", bg: "bg-emerald-500/10" },
            { label: "Critical Stock", value: stats.critical, icon: AlertCircle, color: "text-rose-500", bg: "bg-rose-500/10" },
            { label: "Asset Valuation", value: formatCurrency(stats.totalValue), icon: TrendingUp, color: "text-indigo-500", bg: "bg-indigo-500/10" }
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

        {/* Catalog Control Station */}
        <div className={`rounded-[2.5rem] border overflow-hidden ${isDark ? 'bg-gray-900/50 border-gray-800' : 'bg-white border-gray-100 shadow-2xl shadow-gray-200/40'
          }`}>
          <div className={`p-8 border-b flex flex-col sm:flex-row items-center justify-between gap-6 ${isDark ? 'border-gray-800' : 'border-gray-50'}`}>
            <div className="relative w-full sm:w-96 group">
              <Search className={`absolute left-5 top-1/2 -translate-y-1/2 transition-colors duration-300 ${isDark ? 'text-gray-600 group-focus-within:text-indigo-400' : 'text-gray-400 group-focus-within:text-indigo-500'}`} size={20} />
              <input
                type="text"
                placeholder="Find product by name or SKU..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={`w-full pl-14 pr-6 py-4 rounded-2xl font-semibold transition-all duration-300 focus:outline-none focus:ring-4 ${isDark
                  ? 'bg-gray-800/50 border-transparent focus:bg-gray-800 focus:ring-indigo-500/20 text-white placeholder-gray-600'
                  : 'bg-gray-50 border-transparent focus:bg-white focus:ring-indigo-500/10 text-gray-900 placeholder-gray-400 shadow-inner'
                  }`}
              />
            </div>

            <div className="flex items-center gap-3">
              <button className={`flex items-center gap-2 px-5 py-3 rounded-xl font-bold transition-all ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white' : 'bg-gray-50 text-gray-500 hover:text-gray-900'}`}>
                <Settings2 size={18} />
                Categories
              </button>
              <button className={`p-3 rounded-xl transition-colors ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white' : 'bg-gray-50 text-gray-500 hover:text-gray-900'}`}>
                <Filter size={20} />
              </button>
            </div>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className={`${isDark ? 'bg-gray-800/20' : 'bg-gray-50/50'}`}>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400">Product Line</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400">Inventory Specs</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-right">Unit Price</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">Status</th>
                  <th className="px-8 py-6 text-xs font-black uppercase tracking-widest text-gray-400 text-center">Controls</th>
                </tr>
              </thead>
              <tbody className={`divide-y ${isDark ? 'divide-gray-800' : 'divide-gray-50'}`}>
                {isLoading ? (
                  <tr>
                    <td colSpan={5} className="px-8 py-20 text-center">
                      <div className="flex flex-col items-center gap-4">
                        <div className="h-10 w-10 border-4 border-indigo-600/30 border-t-indigo-600 rounded-full animate-spin" />
                        <p className="font-bold text-gray-500 italic">Accessing central databank...</p>
                      </div>
                    </td>
                  </tr>
                ) : products.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-8 py-20 text-center">
                      <div className="flex flex-col items-center gap-4 opacity-40">
                        <Box size={64} className="text-gray-400" />
                        <p className="text-xl font-black italic tracking-widest">NO ASSETS INDEXED</p>
                      </div>
                    </td>
                  </tr>
                ) : (
                  products.map((p) => (
                    <tr key={p.productId} className={`group transition-all duration-300 ${isDark ? 'hover:bg-gray-800/30' : 'hover:bg-indigo-50/30'}`}>
                      <td className="px-8 py-6">
                        <div className="flex items-center gap-4">
                          <div className={`h-12 w-12 rounded-2xl flex items-center justify-center font-black ${isDark ? 'bg-gray-800' : 'bg-gray-100 text-indigo-500'}`}>
                            {p.name.charAt(0)}
                          </div>
                          <div>
                            <div className="font-black text-lg">{p.name}</div>
                            <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest">{p.categoryName || 'General Asset'}</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-8 py-6">
                        <div className="flex items-center gap-8">
                          <div>
                            <div className="font-black text-sm">{p.sku || '---'}</div>
                            <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest">SKU/REF</div>
                          </div>
                          <div>
                            <div className={`font-black text-sm ${(p.stockBalance || 0) <= p.reorderLevel ? 'text-rose-500' : 'text-emerald-500'}`}>
                              {p.stockBalance || 0} units
                            </div>
                            <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Balance</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-8 py-6 text-right">
                        <div className="font-black text-lg text-indigo-600">{formatCurrency(p.defaultSalePrice)}</div>
                        <div className="text-[10px] font-black text-gray-400 uppercase tracking-widest">Standard MSRP</div>
                      </td>
                      <td className="px-8 py-6 text-center">
                        <span className={`px-4 py-1.5 rounded-xl border text-[10px] font-black uppercase tracking-widest ${p.isActive ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20' : 'bg-rose-500/10 text-rose-500 border-rose-500/20'
                          }`}>
                          {p.isActive ? 'Active' : 'Archived'}
                        </span>
                      </td>
                      <td className="px-8 py-6">
                        <div className="flex items-center justify-center gap-2 scale-90 group-hover:scale-100 transition-transform">
                          <button
                            onClick={() => handleEdit(p)}
                            className={`p-3 rounded-2xl transition-all ${isDark ? 'bg-gray-800 hover:bg-indigo-600' : 'bg-gray-100 hover:bg-indigo-600'} hover:text-white`}
                          >
                            <Edit2 size={18} />
                          </button>
                          <button
                            onClick={() => handleDelete(p.productId)}
                            className={`p-3 rounded-2xl transition-all ${isDark ? 'bg-gray-800 hover:bg-rose-600/20 text-rose-500' : 'bg-gray-100 hover:bg-rose-600/20 text-rose-500'}`}
                          >
                            <Trash2 size={18} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Catalog Modal Entry */}
        <ProductModal
          isOpen={isModalOpen}
          onClose={() => { setIsModalOpen(false); selectProduct(null); }}
          product={selectedProduct}
        />

      </div>
    </MainLayout>
  );
}
