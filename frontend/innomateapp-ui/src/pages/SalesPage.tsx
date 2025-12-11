// components/sales/SalesPage.tsx
import { useState, useEffect } from 'react';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  X,
  Printer,
  Settings,
  DollarSign,
  TrendingUp,
  Package,
  RotateCcw,
  Eye
} from 'lucide-react';
import { motion } from 'framer-motion';
import MainLayout from '@/components/layout/MainLayout.js';
import SaleForm from '@/components/sales/SaleForm.js';
import SaleDetailsModal from '@/components/sales/SaleDetailsModal.js';
import ReturnModal from '@/components/sales/ReturnModal.js';
import InvoicePreview from '@/components/sales/InvoicePreview.js';
import { SaleDTO } from '@/types/sale.js';
import { useTheme } from '@/hooks/useTheme.js';
import { useSaleStore } from '@/store/useSaleStore.js';
import { Link } from 'react-router-dom';

export default function SalesPage() {
  const { isDark } = useTheme();
  const { sales, fetchSales, loading, deleteSale } = useSaleStore();
  const [searchQuery, setSearchQuery] = useState('');
  const [open, setOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [invoiceSale, setInvoiceSale] = useState<SaleDTO | null>(null);
  const [returnSale, setReturnSale] = useState<SaleDTO | null>(null);
  const [viewSale, setViewSale] = useState<SaleDTO | null>(null);
  const [directPrint, setDirectPrint] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;

  // Theme classes
  const theme = {
    // Background colors
    bg: isDark ? 'bg-gray-900' : 'bg-gray-50',
    bgCard: isDark ? 'bg-gray-800' : 'bg-white',
    bgSecondary: isDark ? 'bg-gray-700' : 'bg-gray-50',
    bgHover: isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-100',

    // Text colors
    text: isDark ? 'text-gray-100' : 'text-gray-900',
    textSecondary: isDark ? 'text-gray-400' : 'text-gray-600',
    textMuted: isDark ? 'text-gray-500' : 'text-gray-500',

    // Border colors
    border: isDark ? 'border-gray-700' : 'border-gray-200',
    borderLight: isDark ? 'border-gray-600' : 'border-gray-100',

    // Input colors
    input: isDark
      ? 'bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-400 focus:border-blue-500'
      : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-blue-500',

    // Button colors
    buttonPrimary: 'bg-blue-600 hover:bg-blue-700 text-white',
    buttonSecondary: isDark
      ? 'bg-gray-700 hover:bg-gray-600 text-gray-100'
      : 'bg-gray-200 hover:bg-gray-300 text-gray-700',
    buttonSuccess: 'bg-green-600 hover:bg-green-700 text-white',
  };

  useEffect(() => {
    fetchSales();
  }, [fetchSales]);

  // Reset to first page when search changes
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery]);

  const handleAdd = () => {
    setEditId(null);
    setOpen(true);
  };

  const handleEdit = (id: number) => {
    setEditId(id);
    setOpen(true);
  };

  const handleClose = (refresh = false) => {
    setOpen(false);
    setEditId(null);
    if (refresh) setRefreshKey(prev => prev + 1);
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this sale? This action cannot be undone.')) {
      return;
    }
    await deleteSale(id);
  };

  const handlePrintInvoice = (sale: SaleDTO) => {
    setInvoiceSale(sale);
  };

  const clearSearch = () => {
    setSearchQuery('');
  };

  // Filter and paginate sales
  const filteredSales = sales.filter(sale =>
    sale.customerName?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    sale.saleId.toString().includes(searchQuery) ||
    sale.invoiceNo?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    sale.customerId.toString().includes(searchQuery)
  );

  const totalPages = Math.ceil(filteredSales.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentSales = filteredSales.slice(startIndex, startIndex + itemsPerPage);

  const isEmpty = filteredSales.length === 0;
  const isSearching = searchQuery.length > 0;

  // Calculate summary stats
  const summaryStats = {
    totalSales: filteredSales.length,
    totalRevenue: filteredSales.reduce((sum, sale) => sum + sale.totalAmount, 0),
    totalProfit: filteredSales.reduce((sum, sale) => sum + (sale.totalProfit || 0), 0),
    averageMargin: filteredSales.length > 0
      ? filteredSales.reduce((sum, sale) => sum + (sale.profitMargin || 0), 0) / filteredSales.length
      : 0,
  };

  // Format currency
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  // Format percentage
  const formatPercentage = (value: number) => {
    return `${value.toFixed(1)}%`;
  };

  // Format date
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6">
          <motion.h2
            className={`text-xl font-bold ${theme.text}`}
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
          >
            Sales Management
          </motion.h2>
          <p className={`text-sm mt-1 ${theme.textSecondary}`}>
            Create, view, edit, and manage sales transactions with profit tracking.
          </p>
        </div>

        {/* Summary Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <Package size={20} className="text-blue-600" />
              </div>
              <div>
                <p className={`text-sm ${theme.textSecondary}`}>Total Sales</p>
                <p className={`text-xl font-bold ${theme.text}`}>{summaryStats.totalSales}</p>
              </div>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <DollarSign size={20} className="text-green-600" />
              </div>
              <div>
                <p className={`text-sm ${theme.textSecondary}`}>Total Revenue</p>
                <p className={`text-xl font-bold ${theme.text}`}>{formatCurrency(summaryStats.totalRevenue)}</p>
              </div>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-lg">
                <TrendingUp size={20} className="text-purple-600" />
              </div>
              <div>
                <p className={`text-sm ${theme.textSecondary}`}>Total Profit</p>
                <p className={`text-xl font-bold ${theme.text}`}>{formatCurrency(summaryStats.totalProfit)}</p>
              </div>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-orange-100 rounded-lg">
                <TrendingUp size={20} className="text-orange-600" />
              </div>
              <div>
                <p className={`text-sm ${theme.textSecondary}`}>Avg. Margin</p>
                <p className={`text-xl font-bold ${theme.text}`}>{formatPercentage(summaryStats.averageMargin)}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Printer Options */}
        <div className={`flex items-center gap-4 p-4 rounded-lg mb-6 ${theme.bgCard} ${theme.border}`}>
          <div className={`text-sm font-medium ${theme.text}`}>Printer Options:</div>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={directPrint}
              onChange={(e) => setDirectPrint(e.target.checked)}
              className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <span className={`text-sm ${theme.textSecondary}`}>Direct Print</span>
          </label>
          <button
            onClick={async () => {
              try {
                const device = await (navigator as any).usb?.requestDevice({ filters: [] });
                console.log(device);
                alert('Printer selected (check console)');
              } catch (e) {
                alert('Printer selection failed: ' + e);
              }
            }}
            className={`flex items-center gap-2 px-3 py-1 rounded text-sm transition-colors ${theme.buttonSecondary}`}
          >
            <Printer size={16} />
            Select Printer (WebUSB)
          </button>
        </div>

        {/* Search and Add Button */}
        <div className="flex justify-between items-center mb-6">
          <div className="relative w-80">
            <Search className={`absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 ${theme.textMuted}`} />
            <input
              type="text"
              placeholder="Search by customer, invoice, sale ID..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className={`w-full pl-10 pr-8 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
            />
            {searchQuery && (
              <button
                onClick={clearSearch}
                className={`absolute right-2 top-1/2 transform -translate-y-1/2 p-1 rounded ${isDark ? 'hover:bg-gray-600' : 'hover:bg-gray-200'
                  }`}
              >
                <X size={14} className={theme.textMuted} />
              </button>
            )}
          </div>
          <div className="flex items-center gap-3">

            <button
              onClick={handleAdd}
              className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonPrimary}`}
            >
              <Plus size={20} />
              New Sale
            </button>
          </div>
        </div>

        {/* Table */}
        <div className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}>
          {/* Table Header */}
          <div className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
            <div className="col-span-1">SALE ID</div>
            <div className="col-span-2">INVOICE #</div>
            <div className="col-span-2">CUSTOMER</div>
            <div className="col-span-1">DATE</div>
            <div className="col-span-1">TOTAL</div>
            <div className="col-span-1">PROFIT</div>
            <div className="col-span-1">MARGIN</div>
            <div className="col-span-1">STATUS</div>
            <div className="col-span-2 text-center">ACTIONS</div>
          </div>

          {/* Table Body */}
          <div className="flex-1">
            {loading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
              </div>
            ) : isEmpty ? (
              <div className={`flex flex-col items-center justify-center h-32 ${theme.textMuted}`}>
                {isSearching ? (
                  <>
                    <p>No sales found for "{searchQuery}"</p>
                    <button
                      onClick={clearSearch}
                      className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                    >
                      Clear search
                    </button>
                  </>
                ) : (
                  <p>No sales available</p>
                )}
              </div>
            ) : (
              currentSales.map((sale, index) => (
                <div
                  key={sale.saleId}
                  className={`grid grid-cols-12 gap-4 px-6 py-4 border-b transition-colors ${index % 2 === 0
                    ? isDark ? 'bg-gray-800' : 'bg-white'
                    : isDark ? 'bg-gray-750' : 'bg-gray-50'
                    } ${theme.borderLight} ${theme.bgHover}`}
                >
                  <div className={`col-span-1 text-sm font-medium ${theme.text}`}>
                    {sale.saleId}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {sale.invoiceNo || 'N/A'}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {sale.customerName || `Customer #${sale.customerId}`}
                  </div>
                  <div className={`col-span-1 text-sm ${theme.textSecondary}`}>
                    {formatDate(sale.saleDate)}
                  </div>
                  <div className={`col-span-1 text-sm font-semibold ${theme.text}`}>
                    {formatCurrency(sale.totalAmount)}
                  </div>
                  <div className={`col-span-1 text-sm font-semibold ${(sale.totalProfit || 0) >= 0 ? 'text-green-600' : 'text-red-600'
                    }`}>
                    {formatCurrency(sale.totalProfit || 0)}
                  </div>
                  <div className={`col-span-1 text-sm ${(sale.profitMargin || 0) >= 0 ? 'text-green-600' : 'text-red-600'
                    }`}>
                    {formatPercentage(sale.profitMargin || 0)}
                  </div>
                  <div className="col-span-1">
                    <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${sale.isFullyPaid
                      ? 'bg-green-100 text-green-800'
                      : sale.balanceAmount > 0
                        ? 'bg-yellow-100 text-yellow-800'
                        : 'bg-gray-100 text-gray-800'
                      }`}>
                      {sale.isFullyPaid ? 'Paid' : sale.balanceAmount > 0 ? 'Partial' : 'Pending'}
                    </span>
                  </div>
                  <div className="col-span-2 flex justify-center gap-2">
                    <button
                      onClick={() => setViewSale(sale)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-blue-400 hover:text-blue-300 hover:bg-gray-700' : 'text-blue-600 hover:text-blue-800 hover:bg-gray-100'
                        }`}
                      title="View Details"
                    >
                      <Eye size={18} />
                    </button>
                    <button
                      onClick={() => handlePrintInvoice(sale)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-gray-400 hover:text-gray-300 hover:bg-gray-700' : 'text-gray-600 hover:text-gray-800 hover:bg-gray-100'
                        }`}
                      title="Print Invoice"
                    >
                      <Printer size={18} />
                    </button>
                    <button
                      onClick={() => handleEdit(sale.saleId)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-green-400 hover:text-green-300 hover:bg-gray-700' : 'text-green-600 hover:text-green-800 hover:bg-gray-100'
                        }`}
                      title="Edit"
                    >
                      <Edit2 size={18} />
                    </button>
                    <button
                      onClick={() => handleDelete(sale.saleId)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-red-400 hover:text-red-300 hover:bg-gray-700' : 'text-red-600 hover:text-red-800 hover:bg-gray-100'
                        }`}
                      title="Delete"
                    >
                      <Trash2 size={18} />
                    </button>
                    <button
                      onClick={() => setReturnSale(sale)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-orange-400 hover:text-orange-300 hover:bg-gray-700' : 'text-orange-600 hover:text-orange-800 hover:bg-gray-100'
                        }`}
                      title="Return"
                    >
                      <RotateCcw size={18} />
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>

          {/* Pagination */}
          {!isEmpty && (
            <div className={`px-6 py-4 border-t ${theme.border} ${theme.bgCard}`}>
              <div className="flex justify-between items-center">
                <div className={`text-sm ${theme.textSecondary}`}>
                  Showing {startIndex + 1} to {Math.min(startIndex + itemsPerPage, filteredSales.length)} of {filteredSales.length} results
                </div>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                    disabled={currentPage === 1}
                    className={`p-2 rounded border transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${isDark
                      ? 'border-gray-600 hover:bg-gray-700'
                      : 'border-gray-300 hover:bg-gray-100'
                      }`}
                  >
                    <ChevronLeft size={16} className={theme.text} />
                  </button>
                  <span className={`text-sm px-3 ${theme.text}`}>
                    Page {currentPage} of {totalPages}
                  </span>
                  <button
                    onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                    disabled={currentPage === totalPages}
                    className={`p-2 rounded border transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${isDark
                      ? 'border-gray-600 hover:bg-gray-700'
                      : 'border-gray-300 hover:bg-gray-100'
                      }`}
                  >
                    <ChevronRight size={16} className={theme.text} />
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* View Details Modal */}
        {viewSale && (
          <SaleDetailsModal
            sale={viewSale}
            onClose={() => setViewSale(null)}
          />
        )}

        {/* Add/Edit Sale Dialog */}
        {open && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-6xl h-[95vh] flex flex-col ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <div className="flex justify-between items-center">
                  <h3 className={`text-lg font-semibold ${theme.text}`}>
                    {editId ? 'Edit Sale' : 'Create New Sale'}
                  </h3>
                  <button
                    onClick={() => handleClose(false)}
                    className={`p-2 rounded transition-colors ${isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-100'
                      }`}
                  >
                    <X size={20} className={theme.text} />
                  </button>
                </div>
              </div>

              <div className="flex-1 overflow-auto p-6">
                <SaleForm
                  onClose={(refresh?: boolean) => handleClose(!!refresh)}
                  onShowInvoice={(sale) => {
                    setInvoiceSale(sale);
                    setOpen(false);
                  }}
                />
              </div>
            </div>
          </div>
        )}

        {/* Invoice Preview */}
        {invoiceSale && (
          <InvoicePreview
            sale={invoiceSale}
            onClose={() => setInvoiceSale(null)}
            autoPrint={false}
            directPrint={directPrint}
          />
        )}

        {/* Return Modal */}
        {returnSale && (
          <ReturnModal
            sale={returnSale}
            onClose={(refresh) => {
              setReturnSale(null);
              if (refresh) fetchSales();
            }}
          />
        )}
      </div>
    </MainLayout>
  );
}