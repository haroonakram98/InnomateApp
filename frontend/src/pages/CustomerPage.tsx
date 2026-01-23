// components/customer/CustomerPage.tsx
import { useState, useEffect } from 'react';
import CustomerModal from '@/components/customer/CustomerModal.js';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  X
} from 'lucide-react';
import { CustomerDTO, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer.js';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import {
  useCustomers,
  useCustomersLoading,
  useCustomersError,
  useValidationErrors,
  useCustomerActions
} from '@/store/useCustomerStore.js';

export default function CustomerPage() {
  const { isDark } = useTheme();
  const customers = useCustomers();
  const isLoading = useCustomersLoading();
  const error = useCustomersError();
  const validationErrors = useValidationErrors();
  const {
    fetchCustomers,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    toggleCustomerStatus,
    setValidationErrors,
    clearError
  } = useCustomerActions();

  const [searchQuery, setSearchQuery] = useState('');
  const [editingCustomer, setEditingCustomer] = useState<CustomerDTO | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;

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
  };

  // Load customers on mount
  useEffect(() => {
    fetchCustomers();
  }, [fetchCustomers]);

  // Clear error when component unmounts or search changes
  useEffect(() => {
    return () => {
      clearError();
    };
  }, [clearError]);

  // Reset to first page when search changes
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery]);

  // Handlers using store actions
  const handleDelete = async (customer: CustomerDTO) => {
    if (!confirm(`Are you sure you want to delete "${customer.name}"? This action cannot be undone.`)) {
      return;
    }
    try {
      await deleteCustomer(customer.customerId);
    } catch (error) {
      // Error handled by store
    }
  };

  const startEdit = (customer: CustomerDTO) => {
    setEditingCustomer(customer);
    setIsCreating(false);
  };

  const startCreate = () => {
    setIsCreating(true);
    setEditingCustomer(null);
  };

  const closeModal = () => {
    setEditingCustomer(null);
    setIsCreating(false);
  };

  const clearSearch = () => {
    setSearchQuery('');
  };

  const handleClearError = () => {
    clearError();
  };

  // Filter and paginate customers
  const filteredCustomers = customers.filter(customer =>
    customer.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    customer.email?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    customer.phone?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    customer.address?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const totalPages = Math.ceil(filteredCustomers.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentCustomers = filteredCustomers.slice(startIndex, startIndex + itemsPerPage);

  const isEmpty = filteredCustomers.length === 0;
  const isSearching = searchQuery.length > 0;

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6">
          <h2 className={`text-xl font-bold ${theme.text}`}>Customer Management</h2>
          <p className={`text-sm mt-1 ${theme.textSecondary}`}>
            Add, view, edit, and delete customers.
          </p>
        </div>

        {/* Error Alert - Only show if modal is NOT open */}
        {error && !isCreating && !editingCustomer && (
          <div className={`mb-4 p-4 rounded-lg border ${isDark ? 'bg-red-900/20 border-red-800 text-red-200' : 'bg-red-50 border-red-200 text-red-700'
            }`}>
            <div className="flex justify-between items-start">
              <span>{error}</span>
              <button
                onClick={handleClearError}
                className={`p-1 rounded ${isDark ? 'hover:bg-red-800' : 'hover:bg-red-100'
                  }`}
              >
                <X size={16} />
              </button>
            </div>
          </div>
        )}

        {/* Search and Add Button */}
        <div className="flex justify-between items-center mb-6">
          <div className="relative w-80">
            <Search className={`absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 ${theme.textMuted}`} />
            <input
              type="text"
              placeholder="Search customers..."
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
          <button
            onClick={startCreate}
            className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonPrimary}`}
          >
            <Plus size={20} />
            Add Customer
          </button>
        </div>

        {/* Table */}
        <div className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}>
          {/* Table Header */}
          <div className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
            <div className="col-span-2">CUSTOMER ID</div>
            <div className="col-span-2">NAME</div>
            <div className="col-span-2">EMAIL</div>
            <div className="col-span-2">PHONE</div>
            <div className="col-span-2 text-center">STATUS</div>
            <div className="col-span-2 text-center">ACTIONS</div>
          </div>

          {/* Table Body */}
          <div className="flex-1">
            {isLoading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
              </div>
            ) : isEmpty ? (
              <div className={`flex flex-col items-center justify-center h-32 ${theme.textMuted}`}>
                {isSearching ? (
                  <>
                    <p>No customers found for "{searchQuery}"</p>
                    <button
                      onClick={clearSearch}
                      className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                    >
                      Clear search
                    </button>
                  </>
                ) : (
                  <p>No customers available</p>
                )}
              </div>
            ) : (
              currentCustomers.map((customer, index) => (
                <div
                  key={customer.customerId}
                  className={`grid grid-cols-12 gap-4 px-6 py-4 border-b transition-colors ${index % 2 === 0
                    ? isDark ? 'bg-gray-800' : 'bg-white'
                    : isDark ? 'bg-gray-750' : 'bg-gray-50'
                    } ${theme.borderLight} ${theme.bgHover}`}
                >
                  <div className={`col-span-2 text-sm font-medium ${theme.text}`}>
                    CUST - {customer.customerId}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {customer.name}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.textSecondary}`}>
                    {customer.email || 'N/A'}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.textSecondary}`}>
                    {customer.phone || 'N/A'}
                  </div>
                  <div className="col-span-2 flex justify-center">
                    <button
                      onClick={() => toggleCustomerStatus(customer.customerId)}
                      className={`px-3 py-1 rounded-full text-xs font-medium transition-colors ${customer.isActive
                          ? isDark ? 'bg-green-900/30 text-green-400 hover:bg-green-900/50' : 'bg-green-100 text-green-700 hover:bg-green-200'
                          : isDark ? 'bg-red-900/30 text-red-400 hover:bg-red-900/50' : 'bg-red-100 text-red-700 hover:bg-red-200'
                        }`}
                    >
                      {customer.isActive ? 'Active' : 'Inactive'}
                    </button>
                  </div>
                  <div className="col-span-2 flex justify-center gap-3">
                    <button
                      onClick={() => startEdit(customer)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-green-400 hover:text-green-300 hover:bg-gray-700' : 'text-green-600 hover:text-green-800 hover:bg-gray-100'
                        }`}
                      title="Edit"
                    >
                      <Edit2 size={18} />
                    </button>
                    <button
                      onClick={() => handleDelete(customer)}
                      className={`p-1 rounded transition-colors ${isDark ? 'text-red-400 hover:text-red-300 hover:bg-gray-700' : 'text-red-600 hover:text-red-800 hover:bg-gray-100'
                        }`}
                      title="Delete"
                    >
                      <Trash2 size={18} />
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
                  Showing {startIndex + 1} to {Math.min(startIndex + itemsPerPage, filteredCustomers.length)} of {filteredCustomers.length} results
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

        {/* Create/Edit Modal */}
        {/* Create/Edit Modal */}
        <CustomerModal
          isOpen={isCreating || !!editingCustomer}
          onClose={closeModal}
          customerToEdit={editingCustomer}
        />
      </div>
    </MainLayout>
  );
}