// components/categories/CategoryManager.tsx
import { useState, useEffect } from 'react';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  X
} from 'lucide-react';
import { CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO } from '@/types/category.js';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import { 
  useCategories, 
  useCategoriesLoading, 
  useCategoriesError,
  useCategoryActions 
} from '@/store/useCategoryStore.js';

export default function CategoryPage() {
  const { isDark } = useTheme();
  const categories = useCategories();
  const isLoading = useCategoriesLoading();
  const error = useCategoriesError();
  const { fetchCategories, createCategory, updateCategory, deleteCategory, clearError } = useCategoryActions();

  const [searchQuery, setSearchQuery] = useState('');
  const [editingCategory, setEditingCategory] = useState<CategoryDTO | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;

  const [formData, setFormData] = useState<CreateCategoryDTO>({
    name: '',
    description: '',
  });

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

  // Load categories on mount
  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

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
  const handleCreate = async () => {
    if (!formData.name.trim()) return;
    try {
      await createCategory(formData);
      resetForm();
      setIsCreating(false);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleUpdate = async () => {
    if (!editingCategory || !formData.name.trim()) return;
    
    const updatePayload: UpdateCategoryDTO = {
      name: formData.name,
      description: formData.description
    };

    try {
      await updateCategory(editingCategory.categoryId, updatePayload);
      resetForm();
      setEditingCategory(null);
    } catch (error) {
      // Error handled by store
    }
  };

  const handleDelete = async (category: CategoryDTO) => {
    if (!confirm(`Are you sure you want to delete "${category.name}"? This action cannot be undone.`)) {
      return;
    }
    try {
      await deleteCategory(category.categoryId);
    } catch (error) {
      // Error handled by store
    }
  };

  const resetForm = () => {
    setFormData({ name: '', description: '' });
  };

  const startEdit = (category: CategoryDTO) => {
    setEditingCategory(category);
    setFormData({
      name: category.name,
      description: category.description || ''
    });
    setIsCreating(false);
  };

  const startCreate = () => {
    setIsCreating(true);
    setEditingCategory(null);
    resetForm();
  };

  const cancelEdit = () => {
    setEditingCategory(null);
    setIsCreating(false);
    resetForm();
  };

  const clearSearch = () => {
    setSearchQuery('');
  };

  const handleClearError = () => {
    clearError();
  };

  // Filter and paginate categories
  const filteredCategories = categories.filter(category =>
    category.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    category.description?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const totalPages = Math.ceil(filteredCategories.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentCategories = filteredCategories.slice(startIndex, startIndex + itemsPerPage);

  const isEmpty = filteredCategories.length === 0;
  const isSearching = searchQuery.length > 0;

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6">
          <h2 className={`text-xl font-bold ${theme.text}`}>Category Management</h2>
          <p className={`text-sm mt-1 ${theme.textSecondary}`}>
            Add, view, edit, and delete product categories.
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
              placeholder="Search categories..."
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
            Add Category
          </button>
        </div>

        {/* Table */}
        <div className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}>
          {/* Table Header */}
          <div className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
            <div className="col-span-3">CATEGORY ID</div>
            <div className="col-span-3">CATEGORY NAME</div>
            <div className="col-span-4">DESCRIPTION</div>
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
                    <p>No categories found for "{searchQuery}"</p>
                    <button 
                      onClick={clearSearch}
                      className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                    >
                      Clear search
                    </button>
                  </>
                ) : (
                  <p>No categories available</p>
                )}
              </div>
            ) : (
              currentCategories.map((category, index) => (
                <div
                  key={category.categoryId}
                  className={`grid grid-cols-12 gap-4 px-6 py-4 border-b transition-colors ${
                    index % 2 === 0 
                      ? isDark ? 'bg-gray-800' : 'bg-white' 
                      : isDark ? 'bg-gray-750' : 'bg-gray-50'
                  } ${theme.borderLight} ${theme.bgHover}`}
                >
                  <div className={`col-span-3 text-sm font-medium ${theme.text}`}>
                    CAT - {category.categoryId}
                  </div>
                  <div className={`col-span-3 text-sm ${theme.text}`}>
                    {category.name}
                  </div>
                  <div className={`col-span-4 text-sm ${theme.textSecondary}`}>
                    {category.description || 'No description'}
                  </div>
                  <div className="col-span-2 flex justify-center gap-3">
                    <button
                      onClick={() => startEdit(category)}
                      className={`p-1 rounded transition-colors ${
                        isDark ? 'text-green-400 hover:text-green-300 hover:bg-gray-700' : 'text-green-600 hover:text-green-800 hover:bg-gray-100'
                      }`}
                      title="Edit"
                    >
                      <Edit2 size={18} />
                    </button>
                    <button
                      onClick={() => handleDelete(category)}
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
          {!isEmpty && (
            <div className={`px-6 py-4 border-t ${theme.border} ${theme.bgCard}`}>
              <div className="flex justify-between items-center">
                <div className={`text-sm ${theme.textSecondary}`}>
                  Showing {startIndex + 1} to {Math.min(startIndex + itemsPerPage, filteredCategories.length)} of {filteredCategories.length} results
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
        {(isCreating || editingCategory) && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-md ${theme.bgCard}`}>
              <div className={`p-6 border-b ${theme.border}`}>
                <h3 className={`text-lg font-semibold ${theme.text}`}>
                  {editingCategory ? 'Edit Category' : 'Create New Category'}
                </h3>
              </div>

              <div className="p-6 space-y-4">
                <div>
                  <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                    Category Name *
                  </label>
                  <input
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                    placeholder="Enter category name"
                  />
                </div>

                <div>
                  <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                    Description
                  </label>
                  <textarea
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    rows={3}
                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none ${theme.input}`}
                    placeholder="Enter category description (optional)"
                  />
                </div>
              </div>

              <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                <button
                  onClick={editingCategory ? handleUpdate : handleCreate}
                  disabled={!formData.name.trim()}
                  className={`flex-1 py-2 px-4 rounded-lg transition-colors disabled:opacity-50 ${theme.buttonPrimary}`}
                >
                  {editingCategory ? 'Update Category' : 'Create Category'}
                </button>
                <button
                  onClick={cancelEdit}
                  className={`flex-1 py-2 px-4 rounded-lg border transition-colors ${theme.buttonSecondary}`}
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
}