// components/products/ProductsPage.tsx
import { useState, useEffect } from 'react';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  X,
  Package
} from 'lucide-react';
import { ProductDTO, CreateProductDto, UpdateProductDto } from '@/types/product.js';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import { 
  useProducts, 
  useProductsLoading, 
  useProductsError,
  useProductActions 
} from '@/store/useProductStore.js';
import ProductForm from '@/features/products/ProductForm.js';

export default function ProductsPage() {
  const { isDark } = useTheme();
  const products = useProducts();
  const isLoading = useProductsLoading();
  const error = useProductsError();
  const { fetchProducts, createProduct, updateProduct, deleteProduct, clearError } = useProductActions();
  // NOTE: `deleteProduct` is now treated as "deactivate / toggle active" on the backend.

  const [searchQuery, setSearchQuery] = useState('');
  const [editingProduct, setEditingProduct] = useState<ProductDTO | null>(null);
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

  // Load products on mount
  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  // Clear error when component unmounts
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
  const handleCreate = async (payload: CreateProductDto) => {
    try {
      await createProduct(payload);
      setIsCreating(false);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleUpdate = async (payload: UpdateProductDto) => {
    try {
      await updateProduct(payload);
      setEditingProduct(null);
    } catch (error) {
      // Error handled by store
    }
  };

  // Now this is a "toggle active" / "deactivate" action, not a hard delete.
  const handleToggleActive = async (product: ProductDTO) => {
    const actionText = product.isActive ? 'deactivate' : 'activate';

    if (
      !confirm(
        `Are you sure you want to ${actionText} "${product.name}"?` +
        (product.isActive 
          ? ' It will no longer be available for new sales or purchases, but existing records will remain.'
          : ' It will become available again for new sales and purchases.'
        )
      )
    ) {
      return;
    }

    try {
      // Backend should flip isActive status or deactivate based on current state.
      await deleteProduct(product.productId);
    } catch (error) {
      // Error handled by store
    }
  };

  const startEdit = (product: ProductDTO) => {
    setEditingProduct(product);
    setIsCreating(false);
  };

  const startCreate = () => {
    setIsCreating(true);
    setEditingProduct(null);
  };

  const cancelEdit = () => {
    setEditingProduct(null);
    setIsCreating(false);
  };

  const clearSearch = () => {
    setSearchQuery('');
  };

  const handleClearError = () => {
    clearError();
  };

  // Filter and paginate products
  const filteredProducts = products.filter(product =>
    product.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    product.sku?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    product.categoryName?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const totalPages = Math.ceil(filteredProducts.length / itemsPerPage) || 1;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentProducts = filteredProducts.slice(startIndex, startIndex + itemsPerPage);

  const isEmpty = filteredProducts.length === 0;
  const isSearching = searchQuery.length > 0;

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6">
          <h2 className={`text-xl font-bold ${theme.text}`}>Product Management</h2>
          <p className={`text-sm mt-1 ${theme.textSecondary}`}>
            Add, view, edit, and activate/deactivate products.
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

        {/* Search and Add Button */}
        <div className="flex justify-between items-center mb-6">
          <div className="relative w-80">
            <Search className={`absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 ${theme.textMuted}`} />
            <input
              type="text"
              placeholder="Search products..."
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
          <button
            onClick={startCreate}
            className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonPrimary}`}
          >
            <Plus size={20} />
            Add Product
          </button>
        </div>

        {/* Table */}
        <div className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}>
          {/* Table Header */}
          <div className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
            <div className="col-span-1">ID</div>
            <div className="col-span-2">NAME</div>
            <div className="col-span-2">CATEGORY</div>
            <div className="col-span-1">SKU</div>
            <div className="col-span-1">PRICE</div>
            <div className="col-span-1">REORDER LEVEL</div>
            <div className="col-span-1">ACTIVE</div>
            <div className="col-span-3 text-center">ACTIONS</div>
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
                    <Package size={48} className="mb-2 opacity-50" />
                    <p>No products found for "{searchQuery}"</p>
                    <button 
                      onClick={clearSearch}
                      className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                    >
                      Clear search
                    </button>
                  </>
                ) : (
                  <>
                    <Package size={48} className="mb-2 opacity-50" />
                    <p>No products available</p>
                  </>
                )}
              </div>
            ) : (
              currentProducts.map((product, index) => (
                <div
                  key={product.productId}
                  className={`grid grid-cols-12 gap-4 px-6 py-4 border-b transition-colors ${
                    index % 2 === 0 
                      ? isDark ? 'bg-gray-800' : 'bg-white' 
                      : isDark ? 'bg-gray-750' : 'bg-gray-50'
                  } ${theme.borderLight} ${theme.bgHover}`}
                >
                  <div className={`col-span-1 text-sm font-medium ${theme.text}`}>
                    {product.productId}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {product.name}
                  </div>
                  <div className={`col-span-2 text-sm ${theme.text}`}>
                    {product.categoryName || 'No category'}
                  </div>
                  <div className={`col-span-1 text-sm ${theme.text}`}>
                    {product.sku || 'N/A'}
                  </div>
                  <div className={`col-span-1 text-sm ${theme.text}`}>
                    ${product.defaultSalePrice?.toFixed(2) || '0.00'}
                  </div>
                  <div className={`col-span-1 text-sm ${theme.text}`}>
                    {product.reorderLevel || 0}
                  </div>
                  <div className={`col-span-1 text-sm ${
                    product.isActive ? 'text-green-600' : 'text-red-600'
                  }`}>
                    {product.isActive ? 'Yes' : 'No'}
                  </div>
                  <div className="col-span-3 flex justify-center gap-3">
                    <button
                      onClick={() => startEdit(product)}
                      className={`flex items-center gap-1 px-3 py-1 rounded text-sm transition-colors ${
                        isDark 
                          ? 'text-green-400 hover:text-green-300 hover:bg-gray-700' 
                          : 'text-green-600 hover:text-green-800 hover:bg-gray-100'
                      }`}
                    >
                      <Edit2 size={16} />
                      Edit
                    </button>
                    <button
                      onClick={() => handleToggleActive(product)}
                      className={`flex items-center gap-1 px-3 py-1 rounded text-sm transition-colors ${
                        product.isActive
                          ? (isDark 
                              ? 'text-yellow-300 hover:text-yellow-200 hover:bg-gray-700' 
                              : 'text-yellow-600 hover:text-yellow-800 hover:bg-gray-100')
                          : (isDark
                              ? 'text-blue-300 hover:text-blue-200 hover:bg-gray-700'
                              : 'text-blue-600 hover:text-blue-800 hover:bg-gray-100')
                      }`}
                    >
                      <Trash2 size={16} />
                      {product.isActive ? 'Deactivate' : 'Activate'}
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
                  Showing {startIndex + 1} to {Math.min(startIndex + itemsPerPage, filteredProducts.length)} of {filteredProducts.length} results
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
        {(isCreating || editingProduct) && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-2xl ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <div className="flex items-center justify-between">
                  <h3 className={`text-lg font-semibold ${theme.text}`}>
                    {editingProduct ? 'Edit Product' : 'Create New Product'}
                  </h3>
                  <button
                    onClick={cancelEdit}
                    className={`p-2 rounded-lg transition-colors ${
                      isDark ? 'hover:bg-gray-700' : 'hover:bg-gray-100'
                    }`}
                  >
                    <X size={20} className={theme.text} />
                  </button>
                </div>
              </div>

              <div className="max-h-[80vh] overflow-y-auto">
                <ProductForm
                  product={editingProduct}
                  onCreate={isCreating ? handleCreate : undefined}
                  onUpdate={editingProduct ? handleUpdate : undefined}
                  onCancel={cancelEdit}
                />
              </div>
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
}
