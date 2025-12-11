// src/pages/SupplierPage.tsx
import { useState, useEffect } from 'react';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  X,
  Building,
  Phone,
  Mail,
  MapPin,
  User,
  ToggleLeft,
  ToggleRight,
  BarChart3,
  Filter
} from 'lucide-react';
import { SupplierDTO, CreateSupplierDTO, UpdateSupplierDTO } from '@/types/supplier.js';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import { 
  useSuppliers, 
  useSuppliersLoading, 
  useSuppliersError,
  useSupplierDetail,
  useSupplierStats,
  useSupplierActions 
} from '@/store/useSupplierStore.js';

export default function SupplierPage() {
  const { isDark } = useTheme();
  const suppliers = useSuppliers();
  const isLoading = useSuppliersLoading();
  const error = useSuppliersError();
  const supplierDetail = useSupplierDetail();
  const supplierStats = useSupplierStats();
  const { 
    fetchSuppliers, 
    createSupplier, 
    updateSupplier, 
    deleteSupplier, 
    toggleSupplierStatus,
    fetchSupplierDetail,
    fetchSupplierStats,
    clearError 
  } = useSupplierActions();

  const [searchQuery, setSearchQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'active' | 'inactive'>('all');
  const [isCreating, setIsCreating] = useState(false);
  const [editingSupplier, setEditingSupplier] = useState<SupplierDTO | null>(null);
  const [viewingSupplier, setViewingSupplier] = useState<SupplierDTO | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;

  const [formData, setFormData] = useState<CreateSupplierDTO>({
    name: '',
    email: '',
    phone: '',
    address: '',
    contactPerson: '',
    notes: ''
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

  // Load suppliers on mount
  useEffect(() => {
    fetchSuppliers();
  }, [fetchSuppliers]);

  // Clear error when component unmounts
  useEffect(() => {
    return () => {
      clearError();
    };
  }, [clearError]);

  // Reset to first page when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery, statusFilter]);

  // Filter suppliers
  const filteredSuppliers = suppliers.filter(supplier => {
    const matchesSearch = 
      supplier.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      supplier.email.toLowerCase().includes(searchQuery.toLowerCase()) ||
      supplier.phone.includes(searchQuery) ||
      supplier.contactPerson?.toLowerCase().includes(searchQuery.toLowerCase()) ||
      supplier.address?.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesStatus = 
      statusFilter === 'all' || 
      (statusFilter === 'active' && supplier.isActive) ||
      (statusFilter === 'inactive' && !supplier.isActive);

    return matchesSearch && matchesStatus;
  });

  // Pagination
  const totalPages = Math.ceil(filteredSuppliers.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentSuppliers = filteredSuppliers.slice(startIndex, startIndex + itemsPerPage);

  // Stats
  const activeSuppliersCount = suppliers.filter(s => s.isActive).length;
  const totalSuppliersCount = suppliers.length;

  // Handlers
  const handleCreateSupplier = async () => {
    if (!formData.name || !formData.email || !formData.phone) {
      alert('Please fill in required fields: Name, Email, and Phone');
      return;
    }

    try {
      await createSupplier(formData);
      resetForm();
      setIsCreating(false);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleUpdateSupplier = async () => {
    if (!editingSupplier || !formData.name || !formData.email || !formData.phone) {
      alert('Please fill in required fields: Name, Email, and Phone');
      return;
    }

    try {
      await updateSupplier(editingSupplier.supplierId, {
        ...formData,
        supplierId: editingSupplier.supplierId
      });
      resetForm();
      setEditingSupplier(null);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleDeleteSupplier = async (supplier: SupplierDTO) => {
    if (!confirm(`Are you sure you want to delete "${supplier.name}"? This action cannot be undone.`)) {
      return;
    }

    try {
      await deleteSupplier(supplier.supplierId);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleToggleStatus = async (supplier: SupplierDTO) => {
    try {
      await toggleSupplierStatus(supplier.supplierId);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleViewSupplier = async (supplier: SupplierDTO) => {
    setViewingSupplier(supplier);
    try {
      await fetchSupplierDetail(supplier.supplierId);
      await fetchSupplierStats(supplier.supplierId);
    } catch (error) {
      // Error handled by store
    }
  };

  const resetForm = () => {
    setFormData({
      name: '',
      email: '',
      phone: '',
      address: '',
      contactPerson: '',
      notes: ''
    });
  };

  const startEdit = (supplier: SupplierDTO) => {
    setEditingSupplier(supplier);
    setFormData({
      name: supplier.name,
      email: supplier.email,
      phone: supplier.phone,
      address: supplier.address || '',
      contactPerson: supplier.contactPerson || '',
      notes: supplier.notes || ''
    });
    setIsCreating(false);
  };

  const startCreate = () => {
    setIsCreating(true);
    setEditingSupplier(null);
    resetForm();
  };

  const cancelForm = () => {
    setEditingSupplier(null);
    setIsCreating(false);
    resetForm();
  };

  const clearSearch = () => {
    setSearchQuery('');
  };

  const handleClearError = () => {
    clearError();
  };

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6">
          <h2 className={`text-xl font-bold ${theme.text}`}>Supplier Management</h2>
          <p className={`text-sm mt-1 ${theme.textSecondary}`}>
            Manage your suppliers and their information.
          </p>
        </div>

        {/* Error Alert */}
        {error && (
          <div className={`mb-4 p-4 rounded-lg border ${
            isDark ? 'bg-red-900/20 border-red-800 text-red-200' : 'bg-red-50 border-red-200 text-red-700'
          }`}>
            <div className="flex justify-between items-start">
              <span>{error}</span>
              <button
                onClick={handleClearError}
                className={`p-1 rounded ${
                  isDark ? 'hover:bg-red-800' : 'hover:bg-red-100'
                }`}
              >
                <X size={16} />
              </button>
            </div>
          </div>
        )}

        {/* Stats and Create Button */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div className="flex items-center justify-between">
              <div>
                <p className={`text-sm ${theme.textSecondary}`}>Total Suppliers</p>
                <p className={`text-2xl font-bold ${theme.text}`}>{totalSuppliersCount}</p>
              </div>
              <Building className={`h-8 w-8 ${isDark ? 'text-blue-400' : 'text-blue-600'}`} />
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div>
              <p className={`text-sm ${theme.textSecondary}`}>Active Suppliers</p>
              <p className={`text-2xl font-bold text-green-600`}>{activeSuppliersCount}</p>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <div>
              <p className={`text-sm ${theme.textSecondary}`}>Inactive Suppliers</p>
              <p className={`text-2xl font-bold text-gray-600`}>
                {totalSuppliersCount - activeSuppliersCount}
              </p>
            </div>
          </div>

          <div className={`p-4 rounded-lg border ${theme.bgCard} ${theme.border}`}>
            <button
              onClick={startCreate}
              className={`w-full h-full flex items-center justify-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonPrimary}`}
            >
              <Plus size={20} />
              New Supplier
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
                placeholder="Search suppliers by name, email, phone, or contact person..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={`w-full pl-10 pr-8 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
              />
              {searchQuery && (
                <button
                  onClick={clearSearch}
                  className={`absolute right-2 top-1/2 transform -translate-y-1/2 p-1 rounded ${
                    isDark ? 'hover:bg-gray-600' : 'hover:bg-gray-200'
                  }`}
                >
                  <X size={14} className={theme.textMuted} />
                </button>
              )}
            </div>

            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value as any)}
              className={`px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
            >
              <option value="all">All Statuses</option>
              <option value="active">Active Only</option>
              <option value="inactive">Inactive Only</option>
            </select>

            <div className="md:col-span-2 flex justify-end">
              <button
                onClick={() => fetchSuppliers()}
                className={`flex items-center gap-2 px-4 py-2 rounded-lg border transition-colors ${theme.buttonSecondary}`}
              >
                <Filter size={16} />
                Refresh
              </button>
            </div>
          </div>
        </div>

        {/* Suppliers Table */}
        <div className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}>
          {/* Table Header */}
          <div className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
            <div className="col-span-3">SUPPLIER</div>
            <div className="col-span-2">CONTACT INFO</div>
            <div className="col-span-2">CONTACT PERSON</div>
            <div className="col-span-2">STATUS</div>
            <div className="col-span-3 text-center">ACTIONS</div>
          </div>

          {/* Table Body */}
          <div className="flex-1">
            {isLoading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
              </div>
            ) : currentSuppliers.length === 0 ? (
              <div className={`flex flex-col items-center justify-center h-32 ${theme.textMuted}`}>
                <Building size={48} className="mb-2 opacity-50" />
                <p>No suppliers found</p>
                {searchQuery && (
                  <button 
                    onClick={clearSearch}
                    className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                  >
                    Clear search
                  </button>
                )}
              </div>
            ) : (
              currentSuppliers.map((supplier, index) => (
                <div
                  key={supplier.supplierId}
                  className={`grid grid-cols-12 gap-4 px-6 py-4 border-b transition-colors ${
                    index % 2 === 0 
                      ? isDark ? 'bg-gray-800' : 'bg-white' 
                      : isDark ? 'bg-gray-750' : 'bg-gray-50'
                  } ${theme.borderLight} ${theme.bgHover}`}
                >
                  <div className="col-span-3">
                    <div className={`font-medium ${theme.text}`}>{supplier.name}</div>
                    <div className={`text-xs mt-1 ${theme.textSecondary}`}>
                      {supplier.address || 'No address'}
                    </div>
                  </div>
                  
                  <div className="col-span-2">
                    <div className="flex items-center gap-1 mb-1">
                      <Mail size={14} className={theme.textMuted} />
                      <span className={`text-sm ${theme.text}`}>{supplier.email}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Phone size={14} className={theme.textMuted} />
                      <span className={`text-sm ${theme.text}`}>{supplier.phone}</span>
                    </div>
                  </div>
                  
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {supplier.contactPerson || 'Not specified'}
                  </div>
                  
                  <div className="col-span-2">
                    <button
                      onClick={() => handleToggleStatus(supplier)}
                      className={`flex items-center gap-2 px-3 py-1 rounded-full text-xs font-medium transition-colors ${
                        supplier.isActive
                          ? 'bg-green-100 text-green-800 hover:bg-green-200 dark:bg-green-900 dark:text-green-200'
                          : 'bg-gray-100 text-gray-800 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-300'
                      }`}
                    >
                      {supplier.isActive ? (
                        <ToggleRight size={16} className="text-green-600" />
                      ) : (
                        <ToggleLeft size={16} className="text-gray-500" />
                      )}
                      {supplier.isActive ? 'Active' : 'Inactive'}
                    </button>
                  </div>
                  
                  <div className="col-span-3 flex justify-center gap-2">
                    <button
                      onClick={() => handleViewSupplier(supplier)}
                      className={`p-1 rounded transition-colors ${
                        isDark ? 'text-blue-400 hover:text-blue-300 hover:bg-gray-700' : 'text-blue-600 hover:text-blue-800 hover:bg-gray-100'
                      }`}
                      title="View Details"
                    >
                      <BarChart3 size={18} />
                    </button>
                    
                    <button
                      onClick={() => startEdit(supplier)}
                      className={`p-1 rounded transition-colors ${
                        isDark ? 'text-green-400 hover:text-green-300 hover:bg-gray-700' : 'text-green-600 hover:text-green-800 hover:bg-gray-100'
                      }`}
                      title="Edit"
                    >
                      <Edit2 size={18} />
                    </button>
                    
                    <button
                      onClick={() => handleDeleteSupplier(supplier)}
                      className={`p-1 rounded transition-colors ${
                        isDark ? 'text-red-400 hover:text-red-300 hover:bg-gray-700' : 'text-red-600 hover:text-red-800 hover:bg-gray-100'
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
          {currentSuppliers.length > 0 && (
            <div className={`px-6 py-4 border-t ${theme.border} ${theme.bgCard}`}>
              <div className="flex justify-between items-center">
                <div className={`text-sm ${theme.textSecondary}`}>
                  Showing {startIndex + 1} to {Math.min(startIndex + itemsPerPage, filteredSuppliers.length)} of {filteredSuppliers.length} results
                </div>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                    disabled={currentPage === 1}
                    className={`p-2 rounded border transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${
                      isDark 
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
                    className={`p-2 rounded border transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${
                      isDark 
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
        {(isCreating || editingSupplier) && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-md ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <h3 className={`text-lg font-semibold ${theme.text}`}>
                  {editingSupplier ? 'Edit Supplier' : 'Create New Supplier'}
                </h3>
              </div>

              <div className="p-6 space-y-4">
                <div>
                  <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                    Supplier Name *
                  </label>
                  <input
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                    placeholder="Enter supplier name"
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                      Email *
                    </label>
                    <input
                      type="email"
                      value={formData.email}
                      onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                      className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                      placeholder="email@example.com"
                    />
                  </div>

                  <div>
                    <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                      Phone *
                    </label>
                    <input
                      type="tel"
                      value={formData.phone}
                      onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                      className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                      placeholder="Phone number"
                    />
                  </div>
                </div>

                <div>
                  <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                    Contact Person
                  </label>
                  <input
                    type="text"
                    value={formData.contactPerson}
                    onChange={(e) => setFormData({ ...formData, contactPerson: e.target.value })}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                    placeholder="Contact person name"
                  />
                </div>

                <div>
                  <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                    Address
                  </label>
                  <textarea
                    value={formData.address}
                    onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                    rows={2}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none ${theme.input}`}
                    placeholder="Supplier address"
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
                    placeholder="Additional notes (optional)"
                  />
                </div>
              </div>

              <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                <button
                  onClick={editingSupplier ? handleUpdateSupplier : handleCreateSupplier}
                  disabled={!formData.name || !formData.email || !formData.phone}
                  className={`flex-1 py-2 px-4 rounded-lg transition-colors disabled:opacity-50 ${theme.buttonPrimary}`}
                >
                  {editingSupplier ? 'Update Supplier' : 'Create Supplier'}
                </button>
                <button
                  onClick={cancelForm}
                  className={`flex-1 py-2 px-4 rounded-lg border transition-colors ${theme.buttonSecondary}`}
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        )}

        {/* View Supplier Modal */}
        {viewingSupplier && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-2xl ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <div className="flex justify-between items-center">
                  <h3 className={`text-lg font-semibold ${theme.text}`}>
                    Supplier Details - {viewingSupplier.name}
                  </h3>
                  <button
                    onClick={() => setViewingSupplier(null)}
                    className={`p-1 rounded ${
                      isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-100'
                    }`}
                  >
                    <X size={20} />
                  </button>
                </div>
              </div>

              <div className="p-6 space-y-6 max-h-96 overflow-y-auto">
                {/* Supplier Information */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="space-y-4">
                    <h4 className={`text-md font-semibold ${theme.text}`}>Basic Information</h4>
                    
                    <div className="space-y-3">
                      <div className="flex items-center gap-3">
                        <Building className={`h-5 w-5 ${theme.textSecondary}`} />
                        <div>
                          <p className={`font-semibold ${theme.text}`}>{viewingSupplier.name}</p>
                          <p className={`text-sm ${theme.textSecondary}`}>Supplier Name</p>
                        </div>
                      </div>

                      <div className="flex items-center gap-3">
                        <Mail className={`h-5 w-5 ${theme.textSecondary}`} />
                        <div>
                          <p className={theme.text}>{viewingSupplier.email}</p>
                          <p className={`text-sm ${theme.textSecondary}`}>Email</p>
                        </div>
                      </div>

                      <div className="flex items-center gap-3">
                        <Phone className={`h-5 w-5 ${theme.textSecondary}`} />
                        <div>
                          <p className={theme.text}>{viewingSupplier.phone}</p>
                          <p className={`text-sm ${theme.textSecondary}`}>Phone</p>
                        </div>
                      </div>

                      {viewingSupplier.contactPerson && (
                        <div className="flex items-center gap-3">
                          <User className={`h-5 w-5 ${theme.textSecondary}`} />
                          <div>
                            <p className={theme.text}>{viewingSupplier.contactPerson}</p>
                            <p className={`text-sm ${theme.textSecondary}`}>Contact Person</p>
                          </div>
                        </div>
                      )}

                      <div>
                        <p className={`text-sm font-medium ${theme.text}`}>Status</p>
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-1 ${
                          viewingSupplier.isActive
                            ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                            : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
                        }`}>
                          {viewingSupplier.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </div>
                    </div>
                  </div>

                  <div className="space-y-4">
                    <h4 className={`text-md font-semibold ${theme.text}`}>Additional Information</h4>
                    
                    <div className="space-y-3">
                      {viewingSupplier.address && (
                        <div className="flex items-start gap-3">
                          <MapPin className={`h-5 w-5 mt-0.5 ${theme.textSecondary}`} />
                          <div>
                            <p className={theme.text}>{viewingSupplier.address}</p>
                            <p className={`text-sm ${theme.textSecondary}`}>Address</p>
                          </div>
                        </div>
                      )}

                      {viewingSupplier.notes && (
                        <div>
                          <p className={`text-sm font-medium ${theme.text}`}>Notes</p>
                          <p className={`text-sm ${theme.textSecondary} mt-1`}>{viewingSupplier.notes}</p>
                        </div>
                      )}

                      <div>
                        <p className={`text-sm font-medium ${theme.text}`}>Created</p>
                        <p className={`text-sm ${theme.textSecondary}`}>
                          {new Date(viewingSupplier.createdAt).toLocaleDateString()} at {new Date(viewingSupplier.createdAt).toLocaleTimeString()}
                        </p>
                      </div>

                      {viewingSupplier.updatedAt && (
                        <div>
                          <p className={`text-sm font-medium ${theme.text}`}>Last Updated</p>
                          <p className={`text-sm ${theme.textSecondary}`}>
                            {new Date(viewingSupplier.updatedAt).toLocaleDateString()} at {new Date(viewingSupplier.updatedAt).toLocaleTimeString()}
                          </p>
                        </div>
                      )}
                    </div>
                  </div>
                </div>

                {/* Supplier Stats */}
                {(supplierStats || supplierDetail) && (
                  <div className={`border-t pt-6 ${theme.border}`}>
                    <h4 className={`text-md font-semibold mb-4 ${theme.text}`}>Purchase Statistics</h4>
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                      <div className={`p-3 rounded-lg ${isDark ? 'bg-gray-700' : 'bg-gray-50'}`}>
                        <p className={`text-2xl font-bold ${theme.text}`}>
                          {(supplierDetail?.totalPurchases || supplierStats?.totalPurchases) || 0}
                        </p>
                        <p className={`text-xs ${theme.textSecondary}`}>Total Purchases</p>
                      </div>
                      <div className={`p-3 rounded-lg ${isDark ? 'bg-gray-700' : 'bg-gray-50'}`}>
                        <p className={`text-2xl font-bold text-green-600`}>
                          ${((supplierDetail?.totalPurchaseAmount || supplierStats?.totalPurchaseAmount) || 0).toLocaleString()}
                        </p>
                        <p className={`text-xs ${theme.textSecondary}`}>Total Amount</p>
                      </div>
                      <div className={`p-3 rounded-lg ${isDark ? 'bg-gray-700' : 'bg-gray-50'}`}>
                        <p className={`text-2xl font-bold text-yellow-600`}>
                          {(supplierDetail?.pendingPurchases || supplierStats?.pendingPurchases) || 0}
                        </p>
                        <p className={`text-xs ${theme.textSecondary}`}>Pending</p>
                      </div>
                      <div className={`p-3 rounded-lg ${isDark ? 'bg-gray-700' : 'bg-gray-50'}`}>
                        <p className={`text-sm font-medium ${theme.text}`}>
                          {(supplierDetail?.lastPurchaseDate || supplierStats?.lastPurchaseDate) 
                            ? new Date(supplierDetail?.lastPurchaseDate || supplierStats?.lastPurchaseDate!).toLocaleDateString()
                            : 'Never'
                          }
                        </p>
                        <p className={`text-xs ${theme.textSecondary}`}>Last Purchase</p>
                      </div>
                    </div>
                  </div>
                )}

                {/* Recent Purchases */}
                {supplierDetail?.recentPurchases && supplierDetail.recentPurchases.length > 0 && (
                  <div className={`border-t pt-6 ${theme.border}`}>
                    <h4 className={`text-md font-semibold mb-3 ${theme.text}`}>Recent Purchases</h4>
                    <div className="space-y-2">
                      {supplierDetail.recentPurchases.slice(0, 5).map((purchase) => (
                        <div key={purchase.purchaseId} className={`flex justify-between items-center p-3 rounded ${
                          isDark ? 'bg-gray-700' : 'bg-gray-50'
                        }`}>
                          <div>
                            <div className={`font-medium ${theme.text}`}>{purchase.purchaseNumber}</div>
                            <div className={`text-sm ${theme.textSecondary}`}>
                              {new Date(purchase.purchaseDate).toLocaleDateString()} • {purchase.status}
                            </div>
                          </div>
                          <div className={`text-sm font-semibold ${theme.text}`}>
                            ${purchase.totalAmount.toLocaleString()}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>

              <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                <button
                  onClick={() => {
                    startEdit(viewingSupplier);
                    setViewingSupplier(null);
                  }}
                  className={`flex-1 py-2 px-4 rounded-lg transition-colors ${theme.buttonPrimary}`}
                >
                  Edit Supplier
                </button>
                <button
                  onClick={() => setViewingSupplier(null)}
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