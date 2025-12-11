// src/pages/PurchasePage.tsx
import { useState, useEffect } from 'react';
import {
  Plus,
  Search,
  Eye,
  Truck,
  X,
  ChevronLeft,
  ChevronRight,
  Calendar,
  Filter
} from 'lucide-react';
import { PurchaseDTO, CreatePurchaseDTO, CreatePurchaseDetailDTO } from '@/types/purchase.js';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import {
  usePurchases,
  usePurchasesLoading,
  usePurchasesError,
  usePurchaseActions,
  usePurchaseStore
} from '@/store/usePurchaseStore.js';
import {
  useSuppliers,
  useSuppliersLoading,
  useSupplierActions
} from '@/store/useSupplierStore.js';
import {
  useProducts,
  useProductsLoading,
  useProductActions
} from '@/store/useProductStore.js';
import { useAuthStore } from '@/store/useAuthStore.js';

export default function PurchasePage() {
  const { isDark } = useTheme();
  const purchases = usePurchases();
  const isLoading = usePurchasesLoading();
  const error = usePurchasesError();
  const { fetchPurchases, createPurchase, receivePurchase, cancelPurchase, clearError } = usePurchaseActions();
  const suppliers = useSuppliers();
  const products = useProducts();
  const areSuppliersLoading = useSuppliersLoading();
  const { fetchSuppliers } = useSupplierActions();
  const { fetchProducts } = useProductActions();
  const [searchQuery, setSearchQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'Pending' | 'Received' | 'Cancelled'>('all');
  const [dateRange, setDateRange] = useState({
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0]
  });
  const [isCreating, setIsCreating] = useState(false);
  const [viewingPurchase, setViewingPurchase] = useState<PurchaseDTO | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [suppliersLoaded, setSuppliersLoaded] = useState(false);
  const itemsPerPage = 8;
  const { user } = useAuthStore();

  const { getNextPurchaseNumber } = usePurchaseActions();

  // Fetch invoice number when modal opens
  useEffect(() => {
    if (isCreating) {
      getNextPurchaseNumber().then(no => {
        if (no) {
          setFormData(prev => ({ ...prev, invoiceNo: no }));
        }
      });
    }
  }, [isCreating, getNextPurchaseNumber]);

  const [newDetail, setNewDetail] = useState<CreatePurchaseDetailDTO>({
    productId: 0,
    quantity: 0,
    unitCost: 0,
    batchNo: '',
    expiryDate: ''
  });

  // Theme classes
  const theme = {
    bg: isDark ? 'bg-gray-900' : 'bg-gray-50',
    bgCard: isDark ? 'bg-gray-800' : 'bg-white',
    bgSecondary: isDark ? 'bg-gray-700' : 'bg-gray-50',
    bgHover: isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-100',

    text: isDark ? 'text-gray-100' : 'text-gray-900',
    textSecondary: isDark ? 'text-gray-400' : 'text-gray-600',
    textMuted: isDark ? 'text-gray-500' : 'text-gray-500',

    border: isDark ? 'border-gray-700' : 'border-gray-200',
    borderLight: isDark ? 'border-gray-600' : 'border-gray-100',

    input: isDark
      ? 'bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-400 focus:border-blue-500'
      : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-blue-500',

    buttonPrimary: 'bg-blue-600 hover:bg-blue-700 text-white',
    buttonSecondary: isDark
      ? 'bg-gray-700 hover:bg-gray-600 text-gray-100'
      : 'bg-gray-200 hover:bg-gray-300 text-gray-700',
  };

  // Status badge colors
  const statusColors = {
    Pending: isDark ? 'bg-yellow-900 text-yellow-200' : 'bg-yellow-100 text-yellow-800',
    Received: isDark ? 'bg-green-900 text-green-200' : 'bg-green-100 text-green-800',
    Cancelled: isDark ? 'bg-red-900 text-red-200' : 'bg-red-100 text-red-800'
  };

  // Load purchases on mount
  useEffect(() => {
    const loadData = async () => {
      try {
        await Promise.all([
          fetchPurchases(),
          fetchSuppliers(),
          fetchProducts()
        ]);
        setSuppliersLoaded(true);
      } catch (error) {
        console.error('Failed to load data:', error);
      }
    };

    loadData();
  }, [fetchPurchases, fetchSuppliers, fetchProducts]);

  const [formData, setFormData] = useState<CreatePurchaseDTO>({
    supplierId: 0,
    purchaseDate: new Date().toISOString().split('T')[0],
    notes: '',
    purchaseDetails: [],
    invoiceNo: '',
    createdBy: user?.id || 1,
  });

  const activeSuppliers = suppliers.filter(supplier => supplier.isActive);

  const renderSupplierDropdown = () => {
    if (areSuppliersLoading && !suppliersLoaded) {
      return (
        <select disabled className={`w-full px-3 py-2 border rounded-lg ${theme.input}`}>
          <option>Loading suppliers...</option>
        </select>
      );
    }

    if (activeSuppliers.length === 0) {
      return (
        <select disabled className={`w-full px-3 py-2 border rounded-lg ${theme.input}`}>
          <option>No active suppliers available</option>
        </select>
      );
    }

    return (
      <select
        value={formData.supplierId}
        onChange={(e) => setFormData({ ...formData, supplierId: parseInt(e.target.value) })}
        className={`w-full px-3 py-2 border rounded-lg ${theme.input}`}
      >
        <option value={0}>Select Supplier</option>
        {activeSuppliers.map(supplier => (
          <option key={supplier.supplierId} value={supplier.supplierId}>
            {supplier.name} {supplier.phone ? `- ${supplier.phone}` : ''}
          </option>
        ))}
      </select>
    );
  };

  // Clear error when component unmounts
  useEffect(() => {
    return () => {
      clearError();
    };
  }, [clearError]);

  // Reset to first page when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery, statusFilter, dateRange]);

  // Filter purchases
  const filteredPurchases = purchases.filter(purchase => {
    const matchesSearch = purchase.purchaseNumber.toLowerCase().includes(searchQuery.toLowerCase()) ||
      purchase.supplierName?.toLowerCase().includes(searchQuery.toLowerCase());
    const matchesStatus = statusFilter === 'all' || purchase.status === statusFilter;
    const purchaseDate = new Date(purchase.purchaseDate);
    const startDate = new Date(dateRange.startDate);
    const endDate = new Date(dateRange.endDate);
    endDate.setHours(23, 59, 59, 999);

    const matchesDate = purchaseDate >= startDate && purchaseDate <= endDate;

    return matchesSearch && matchesStatus && matchesDate;
  });

  // Pagination
  const totalPages = Math.ceil(filteredPurchases.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentPurchases = filteredPurchases.slice(startIndex, startIndex + itemsPerPage);

  // Calculate total for each line item and overall total
  const calculateLineTotal = (quantity: number, unitCost: number) => {
    return quantity * unitCost;
  };

  const calculatePurchaseTotal = () => {
    return formData.purchaseDetails.reduce((total, detail) => {
      return total + calculateLineTotal(detail.quantity, detail.unitCost);
    }, 0);
  };

  // Handlers
  const handleCreatePurchase = async () => {
    // Validate required fields
    if (!formData.supplierId) {
      alert('Please select a supplier');
      return;
    }

    if (!formData.invoiceNo.trim()) {
      alert('Please enter an invoice number');
      return;
    }

    if (formData.purchaseDetails.length === 0) {
      alert('Please add at least one purchase detail');
      return;
    }

    try {
      await createPurchase(formData);
      resetForm();
      setIsCreating(false);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleReceivePurchase = async (purchase: PurchaseDTO) => {
    if (!confirm(`Receive purchase ${purchase.purchaseNumber}? This will update stock levels.`)) {
      return;
    }

    try {
      await receivePurchase(purchase.purchaseId);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleCancelPurchase = async (purchase: PurchaseDTO) => {
    if (!confirm(`Cancel purchase ${purchase.purchaseNumber}?`)) {
      return;
    }

    try {
      await cancelPurchase(purchase.purchaseId);
    } catch (error) {
      // Error handled by store
    }
  };

  const addPurchaseDetail = () => {
    if (!newDetail.productId || newDetail.quantity <= 0 || newDetail.unitCost <= 0) {
      alert('Please fill in all detail fields with valid values');
      return;
    }

    // Add the detail to formData
    setFormData(prev => ({
      ...prev,
      purchaseDetails: [...prev.purchaseDetails, newDetail]
    }));

    // Reset new detail form
    setNewDetail({
      productId: 0,
      quantity: 0,
      unitCost: 0,
      batchNo: '',
      expiryDate: ''
    });
  };

  const removePurchaseDetail = (index: number) => {
    setFormData(prev => ({
      ...prev,
      purchaseDetails: prev.purchaseDetails.filter((_, i) => i !== index)
    }));
  };

  const resetForm = () => {
    setFormData({
      supplierId: 0,
      purchaseDate: new Date().toISOString().split('T')[0],
      notes: '',
      purchaseDetails: [],
      createdBy: user?.id || 1,
      invoiceNo: ''
    });
  };

  const getStatusCount = (status: string) => {
    return purchases.filter(p => p.status === status).length;
  };

  const getTotalAmount = () => {
    return purchases.reduce((total, purchase) => total + purchase.totalAmount, 0);
  };

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6">
          <h2 className={`text-xl font-bold ${theme.text}`}>Purchase Management</h2>
          <p className={`text-sm mt-1 ${theme.textSecondary}`}>
            Create, view, and manage purchase orders.
          </p>
        </div>

        {/* Error Alert */}
        {error && (
          <div className={`mb-4 p-4 rounded-lg border ${isDark ? 'bg-red-900/20 border-red-800 text-red-200' : 'bg-red-50 border-red-200 text-red-700'
            }`}>
            <div className="flex justify-between items-start">
              <span>{error}</span>
              <button
                onClick={clearError}
                className={`p-1 rounded ${isDark ? 'hover:bg-red-800' : 'hover:bg-red-100'
                  }`}
              >
                <X size={16} />
              </button>
            </div>
          </div>
        )}

        {/* Stats and Filters */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div className="flex items-center justify-between">
              <div>
                <p className={`text-sm ${theme.textSecondary}`}>Total Purchases</p>
                <p className={`text-2xl font-bold ${theme.text}`}>{purchases.length}</p>
              </div>
              <Truck className={`h-8 w-8 ${isDark ? 'text-blue-400' : 'text-blue-600'}`} />
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div>
              <p className={`text-sm ${theme.textSecondary}`}>Pending</p>
              <p className={`text-2xl font-bold text-yellow-600`}>{getStatusCount('Pending')}</p>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div>
              <p className={`text-sm ${theme.textSecondary}`}>Total Amount</p>
              <p className={`text-2xl font-bold text-green-600`}>
                ${getTotalAmount().toLocaleString()}
              </p>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <button
              onClick={() => setIsCreating(true)}
              className={`w-full h-full flex items-center justify-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonPrimary}`}
            >
              <Plus size={20} />
              New Purchase
            </button>
          </div>
        </div>

        {/* Search and Filters */}
        <div className={`p-4 rounded-lg border mb-6 ${theme.bgCard} ${theme.border}`}>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="relative">
              <Search className={`absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 ${theme.textMuted}`} />
              <input
                type="text"
                placeholder="Search purchases..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
              />
            </div>

            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value as any)}
              className={`px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
            >
              <option value="all">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Received">Received</option>
              <option value="Cancelled">Cancelled</option>
            </select>

            <input
              type="date"
              value={dateRange.startDate}
              onChange={(e) => setDateRange(prev => ({ ...prev, startDate: e.target.value }))}
              className={`px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
            />

            <input
              type="date"
              value={dateRange.endDate}
              onChange={(e) => setDateRange(prev => ({ ...prev, endDate: e.target.value }))}
              className={`px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
            />
          </div>
        </div>

        {/* Purchases Table */}
        <div className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}>
          {/* Table Header */}
          <div className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
            <div className="col-span-2">PURCHASE #</div>
            <div className="col-span-2">SUPPLIER</div>
            <div className="col-span-2">DATE</div>
            <div className="col-span-1">ITEMS</div>
            <div className="col-span-2">TOTAL AMOUNT</div>
            <div className="col-span-2">STATUS</div>
            <div className="col-span-1 text-center">ACTIONS</div>
          </div>

          {/* Table Body */}
          <div className="flex-1">
            {isLoading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
              </div>
            ) : currentPurchases.length === 0 ? (
              <div className={`flex flex-col items-center justify-center h-32 ${theme.textMuted}`}>
                <Truck size={48} className="mb-2 opacity-50" />
                <p>No purchases found</p>
                <button
                  onClick={() => setIsCreating(true)}
                  className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                >
                  Create your first purchase
                </button>
              </div>
            ) : (
              currentPurchases.map((purchase, index) => (
                <div
                  key={purchase.purchaseId}
                  className={`grid grid-cols-12 gap-4 px-6 py-4 border-b transition-colors ${index % 2 === 0
                    ? isDark ? 'bg-gray-800' : 'bg-white'
                    : isDark ? 'bg-gray-750' : 'bg-gray-50'
                    } ${theme.borderLight} ${theme.bgHover}`}
                >
                  <div className={`col-span-2 text-sm font-medium ${theme.text}`}>
                    {purchase.purchaseNumber}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {purchase.supplierName}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {new Date(purchase.purchaseDate).toLocaleDateString()}
                  </div>
                  <div className={`col-span-1 text-sm ${theme.text}`}>
                    {purchase.purchaseDetails.length}
                  </div>
                  <div className={`col-span-2 text-sm font-semibold ${theme.text}`}>
                    ${purchase.totalAmount.toLocaleString()}
                  </div>
                  <div className="col-span-2">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${statusColors[purchase.status]}`}>
                      {purchase.status}
                    </span>
                  </div>
                  <div className="col-span-1 flex justify-center gap-2">
                    <button
                      onClick={() => setViewingPurchase(purchase)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-blue-400 hover:text-blue-300 hover:bg-gray-700' : 'text-blue-600 hover:text-blue-800 hover:bg-gray-100'
                        }`}
                      title="View Details"
                    >
                      <Eye size={18} />
                    </button>

                    {purchase.status === 'Pending' && (
                      <>
                        <button
                          onClick={() => handleReceivePurchase(purchase)}
                          className={`p-1 rounded transition-colors ${isDark ? 'text-green-400 hover:text-green-300 hover:bg-gray-700' : 'text-green-600 hover:text-green-800 hover:bg-gray-100'
                            }`}
                          title="Receive Purchase"
                        >
                          <Truck size={18} />
                        </button>
                        <button
                          onClick={() => handleCancelPurchase(purchase)}
                          className={`p-1 rounded transition-colors ${isDark ? 'text-red-400 hover:text-red-300 hover:bg-gray-700' : 'text-red-600 hover:text-red-800 hover:bg-gray-100'
                            }`}
                          title="Cancel Purchase"
                        >
                          <X size={18} />
                        </button>
                      </>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>

          {/* Pagination */}
          {currentPurchases.length > 0 && (
            <div className={`px-6 py-4 border-t ${theme.border} ${theme.bgCard}`}>
              <div className="flex justify-between items-center">
                <div className={`text-sm ${theme.textSecondary}`}>
                  Showing {startIndex + 1} to {Math.min(startIndex + itemsPerPage, filteredPurchases.length)} of {filteredPurchases.length} results
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

        {/* Create Purchase Modal */}
        {isCreating && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-4xl ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <h3 className={`text-lg font-semibold ${theme.text}`}>
                  Create New Purchase
                </h3>
              </div>

              <div className="p-6 space-y-4 max-h-96 overflow-y-auto">
                {/* Basic Information */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                      Supplier *
                    </label>
                    {renderSupplierDropdown()}
                  </div>

                  <div>
                    <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                      Purchase Date *
                    </label>
                    <input
                      type="date"
                      value={formData.purchaseDate}
                      onChange={(e) => setFormData({ ...formData, purchaseDate: e.target.value })}
                      className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                    />
                  </div>

                  {/* Invoice Number Field */}
                  <div>
                    <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                      Invoice No *
                    </label>
                    <input
                      type="text"
                      value={formData.invoiceNo}
                      onChange={(e) => setFormData({ ...formData, invoiceNo: e.target.value })}
                      className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                      placeholder="Loading..."
                    />
                  </div>

                  <div>
                    <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                      Notes
                    </label>
                    <textarea
                      value={formData.notes}
                      onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                      rows={2}
                      className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none ${theme.input}`}
                      placeholder="Enter purchase notes (optional)"
                    />
                  </div>
                </div>

                {/* Purchase Details */}
                <div className="border-t pt-4">
                  <h4 className={`text-md font-semibold mb-3 ${theme.text}`}>Purchase Details</h4>

                  {/* Add Detail Form */}
                  <div className="grid grid-cols-6 gap-2 mb-4 p-3 rounded-lg bg-gray-50 dark:bg-gray-700">
                    <select
                      value={newDetail.productId}
                      onChange={(e) => {
                        const productId = parseInt(e.target.value);
                        setNewDetail({ ...newDetail, productId });

                        if (productId !== 0) {
                          // Fetch new batch number when product is selected
                          const { getNextBatchNumber } = usePurchaseStore.getState().actions;
                          getNextBatchNumber().then(batchNo => {
                            if (batchNo) {
                              setNewDetail(prev => ({ ...prev, batchNo }));
                            }
                          });
                        }
                      }}
                      className={`px-2 py-1 border rounded focus:outline-none focus:ring-1 focus:ring-blue-500 ${theme.input}`}
                    >
                      <option value={0}>Product</option>
                      {products.map(product => (
                        <option key={product.productId} value={product.productId}>
                          {product.name}
                        </option>
                      ))}
                    </select>

                    <input
                      type="number"
                      placeholder="Qty"
                      value={newDetail.quantity || ''}
                      onChange={(e) => setNewDetail({ ...newDetail, quantity: parseFloat(e.target.value) })}
                      className={`px-2 py-1 border rounded focus:outline-none focus:ring-1 focus:ring-blue-500 ${theme.input}`}
                      step="0.01"
                      min="0"
                    />

                    <input
                      type="number"
                      placeholder="Unit Cost"
                      value={newDetail.unitCost || ''}
                      onChange={(e) => setNewDetail({ ...newDetail, unitCost: parseFloat(e.target.value) })}
                      className={`px-2 py-1 border rounded focus:outline-none focus:ring-1 focus:ring-blue-500 ${theme.input}`}
                      step="0.01"
                      min="0"
                    />

                    <input
                      type="text"
                      placeholder="Batch No"
                      value={newDetail.batchNo}
                      onChange={(e) => setNewDetail({ ...newDetail, batchNo: e.target.value })}
                      className={`px-2 py-1 border rounded focus:outline-none focus:ring-1 focus:ring-blue-500 ${theme.input}`}
                    />

                    <input
                      type="date"
                      placeholder="Expiry Date"
                      value={newDetail.expiryDate || ''}
                      onChange={(e) => setNewDetail({ ...newDetail, expiryDate: e.target.value })}
                      className={`px-2 py-1 border rounded focus:outline-none focus:ring-1 focus:ring-blue-500 ${theme.input}`}
                    />

                    <button
                      onClick={addPurchaseDetail}
                      className="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
                    >
                      Add
                    </button>
                  </div>

                  {/* Details List with Line Totals */}
                  {formData.purchaseDetails.length > 0 ? (
                    <div className="space-y-2">
                      {formData.purchaseDetails.map((detail, index) => {
                        const product = products.find(p => p.productId === detail.productId);
                        const lineTotal = calculateLineTotal(detail.quantity, detail.unitCost);

                        return (
                          <div key={index} className={`flex items-center justify-between p-3 rounded ${isDark ? 'bg-gray-700' : 'bg-gray-50'}`}>
                            <div className="flex-1">
                              <div className="flex justify-between items-start">
                                <span className={`font-medium ${theme.text}`}>{product?.name}</span>
                                <span className={`font-semibold ${theme.text}`}>
                                  ${lineTotal.toFixed(2)}
                                </span>
                              </div>
                              <div className={`text-sm ${theme.textSecondary} mt-1`}>
                                <span>Qty: {detail.quantity} × ${detail.unitCost}</span>
                                {detail.batchNo && (
                                  <span className="ml-3">Batch: {detail.batchNo}</span>
                                )}
                                {detail.expiryDate && (
                                  <span className="ml-3">Expiry: {new Date(detail.expiryDate).toLocaleDateString()}</span>
                                )}
                              </div>
                            </div>
                            <button
                              onClick={() => removePurchaseDetail(index)}
                              className={`p-1 rounded ml-2 ${isDark ? 'hover:bg-gray-600 text-red-400' : 'hover:bg-gray-200 text-red-600'
                                }`}
                            >
                              <X size={16} />
                            </button>
                          </div>
                        );
                      })}

                      {/* Purchase Total */}
                      <div className={`pt-3 border-t ${theme.border}`}>
                        <div className="flex justify-between items-center">
                          <span className={`text-lg font-semibold ${theme.text}`}>Total Purchase Amount:</span>
                          <span className={`text-lg font-bold ${theme.text}`}>
                            ${calculatePurchaseTotal().toFixed(2)}
                          </span>
                        </div>
                      </div>
                    </div>
                  ) : (
                    <div className={`text-center py-4 ${theme.textMuted}`}>
                      No purchase details added yet
                    </div>
                  )}
                </div>
              </div>

              <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                <button
                  onClick={handleCreatePurchase}
                  disabled={formData.supplierId === 0 || formData.purchaseDetails.length === 0 || !formData.invoiceNo.trim()}
                  className={`flex-1 py-2 px-4 rounded-lg transition-colors disabled:opacity-50 ${theme.buttonPrimary}`}
                >
                  Create Purchase
                </button>
                <button
                  onClick={() => {
                    setIsCreating(false);
                    resetForm();
                  }}
                  className={`flex-1 py-2 px-4 rounded-lg border transition-colors ${theme.buttonSecondary}`}
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        )}

        {/* View Purchase Modal */}
        {viewingPurchase && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-4xl ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <div className="flex justify-between items-center">
                  <h3 className={`text-lg font-semibold ${theme.text}`}>
                    Purchase Details - {viewingPurchase.purchaseNumber}
                  </h3>
                  <button
                    onClick={() => setViewingPurchase(null)}
                    className={`p-1 rounded ${isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-100'
                      }`}
                  >
                    <X size={20} />
                  </button>
                </div>
              </div>

              <div className="p-6 space-y-4 max-h-96 overflow-y-auto">
                {/* Purchase Information */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className={`block text-sm font-medium ${theme.textSecondary}`}>Supplier</label>
                    <p className={`text-sm ${theme.text}`}>{viewingPurchase.supplierName}</p>
                  </div>
                  <div>
                    <label className={`block text-sm font-medium ${theme.textSecondary}`}>Purchase Date</label>
                    <p className={`text-sm ${theme.text}`}>
                      {new Date(viewingPurchase.purchaseDate).toLocaleDateString()}
                    </p>
                  </div>
                  <div>
                    <label className={`block text-sm font-medium ${theme.textSecondary}`}>Status</label>
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${statusColors[viewingPurchase.status]}`}>
                      {viewingPurchase.status}
                    </span>
                  </div>
                  <div>
                    <label className={`block text-sm font-medium ${theme.textSecondary}`}>Total Amount</label>
                    <p className={`text-sm font-semibold ${theme.text}`}>
                      ${viewingPurchase.totalAmount.toLocaleString()}
                    </p>
                  </div>
                </div>

                {viewingPurchase.notes && (
                  <div>
                    <label className={`block text-sm font-medium ${theme.textSecondary}`}>Notes</label>
                    <p className={`text-sm ${theme.text}`}>{viewingPurchase.notes}</p>
                  </div>
                )}

                {/* Purchase Details */}
                <div className="border-t pt-4">
                  <h4 className={`text-md font-semibold mb-3 ${theme.text}`}>Purchase Items</h4>
                  <div className="space-y-2">
                    {viewingPurchase.purchaseDetails.map((detail, index) => (
                      <div key={index} className={`flex justify-between items-center p-3 rounded ${isDark ? 'bg-gray-700' : 'bg-gray-50'
                        }`}>
                        <div>
                          <div className={`font-medium ${theme.text}`}>
                            {detail.productName} ({detail.productCode})
                          </div>
                          <div className={`text-sm ${theme.textSecondary}`}>
                            Quantity: {detail.quantity} | Unit Cost: ${detail.unitCost} | Total: ${detail.totalCost}
                            {detail.remainingQty !== undefined && ` | Remaining: ${detail.remainingQty}`}
                            {detail.batchNo && ` | Batch: ${detail.batchNo}`}
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>

              <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                {viewingPurchase.status === 'Pending' && (
                  <>
                    <button
                      onClick={() => {
                        handleReceivePurchase(viewingPurchase);
                        setViewingPurchase(null);
                      }}
                      className={`flex-1 py-2 px-4 rounded-lg transition-colors bg-green-600 hover:bg-green-700 text-white`}
                    >
                      Receive Purchase
                    </button>
                    <button
                      onClick={() => {
                        handleCancelPurchase(viewingPurchase);
                        setViewingPurchase(null);
                      }}
                      className={`flex-1 py-2 px-4 rounded-lg transition-colors bg-red-600 hover:bg-red-700 text-white`}
                    >
                      Cancel Purchase
                    </button>
                  </>
                )}
                <button
                  onClick={() => setViewingPurchase(null)}
                  className={`flex-1 py-2 px-4 rounded-lg border transition-colors ${theme.buttonSecondary}`}
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
}