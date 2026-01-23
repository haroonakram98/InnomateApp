import { useState, useEffect, useCallback } from 'react';
import {
  Plus,
  Search,
  Edit2,
  Trash2,
  ChevronLeft,
  ChevronRight,
  RefreshCw,
  MoreVertical,
  CheckCircle2,
  XCircle,
  LayoutGrid,
  Filter
} from 'lucide-react';
import {
  useCategoryStore,
  useCategoryActions
} from '@/store/useCategoryStore.js';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import CategoryModal from '@/components/inventory/CategoryModal.js';
import { CategoryDTO } from '@/types/category.js';

export default function CategoryPage() {
  const { isDark } = useTheme();
  const categories = useCategoryStore((state) => state.categories);
  const isLoading = useCategoryStore((state) => state.isLoading);
  const error = useCategoryStore((state) => state.error);
  const { fetchCategories, deleteCategory, toggleStatus, clearError } = useCategoryActions();

  // Clear errors on unmount
  useEffect(() => {
    return () => clearError();
  }, [clearError]);

  const [searchQuery, setSearchQuery] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<CategoryDTO | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;

  // Debounced search
  useEffect(() => {
    const timer = setTimeout(() => {
      fetchCategories(searchQuery);
    }, 500);
    return () => clearTimeout(timer);
  }, [searchQuery, fetchCategories]);

  const handleCreate = () => {
    setSelectedCategory(null);
    setIsModalOpen(true);
  };

  const handleEdit = (category: CategoryDTO) => {
    setSelectedCategory(category);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: number, name: string) => {
    if (window.confirm(`Are you sure you want to delete category "${name}"?`)) {
      try {
        await deleteCategory(id);
      } catch (err) {
        // Error handled in store
      }
    }
  };

  const totalPages = Math.ceil(categories.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentCategories = categories.slice(startIndex, startIndex + itemsPerPage);

  const containerClasses = `min-h-screen p-8 transition-colors duration-300 ${isDark ? 'bg-gray-950' : 'bg-gray-50/50'}`;
  const cardClasses = `rounded-3xl border shadow-xl overflow-hidden ${isDark ? 'bg-gray-900/50 border-gray-800 backdrop-blur-xl' : 'bg-white border-gray-100 shadow-gray-200/50'
    }`;

  return (
    <MainLayout>
      <div className={containerClasses}>
        {/* Header Section */}
        <div className="max-w-7xl mx-auto mb-8">
          <div className="flex flex-col md:flex-row md:items-center justify-between gap-6">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-2.5 bg-blue-600/10 rounded-2xl text-blue-600">
                  <LayoutGrid size={24} />
                </div>
                <h1 className={`text-3xl font-bold tracking-tight ${isDark ? 'text-white' : 'text-gray-900'}`}>
                  Categories
                </h1>
              </div>
              <p className={isDark ? 'text-gray-400' : 'text-gray-500'}>
                Organize your product inventory with custom categories.
              </p>
            </div>

            <button
              onClick={handleCreate}
              className="group flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-6 py-3.5 rounded-2xl font-semibold shadow-lg shadow-blue-600/20 active:scale-[0.98] transition-all duration-200"
            >
              <Plus size={20} className="group-hover:rotate-90 transition-transform duration-300" />
              Add Category
            </button>
          </div>
        </div>

        {/* Filters and Utilities */}
        <div className="max-w-7xl mx-auto mb-6">
          <div className="flex flex-wrap items-center justify-between gap-4">
            <div className={`relative w-full max-w-md group`}>
              <Search className={`absolute left-4 top-1/2 -translate-y-1/2 transition-colors ${isDark ? 'text-gray-500 group-focus-within:text-blue-500' : 'text-gray-400 group-focus-within:text-blue-600'
                }`} size={20} />
              <input
                type="text"
                placeholder="Search by name or description..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={`w-full pl-12 pr-4 py-3.5 rounded-2xl border transition-all duration-200 outline-none ${isDark
                  ? 'bg-gray-900/50 border-gray-800 text-white focus:ring-4 focus:ring-blue-500/10 focus:border-blue-500'
                  : 'bg-white border-gray-200 text-gray-900 focus:ring-4 focus:ring-blue-600/5 focus:border-blue-600'
                  }`}
              />
            </div>

            <div className="flex items-center gap-3">
              <button
                onClick={() => fetchCategories(searchQuery)}
                className={`p-3 rounded-2xl border transition-all duration-200 ${isDark ? 'bg-gray-900 border-gray-800 text-gray-400 hover:text-white hover:bg-gray-800' : 'bg-white border-gray-200 text-gray-500 hover:text-gray-900 hover:bg-gray-50'
                  }`}
              >
                <RefreshCw size={20} className={isLoading ? 'animate-spin' : ''} />
              </button>
              <div className={`h-8 w-[1px] ${isDark ? 'bg-gray-800' : 'bg-gray-200'}`} />
              <span className={`text-sm font-medium ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
                {categories.length} Categories Total
              </span>
            </div>
          </div>
        </div>

        {/* Content Table */}
        <div className="max-w-7xl mx-auto">
          <div className={cardClasses}>
            <div className="overflow-x-auto">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className={isDark ? 'bg-gray-800/50 text-gray-300' : 'bg-gray-50 text-gray-600'}>
                    <th className="px-8 py-5 text-xs font-bold uppercase tracking-wider">Category</th>
                    <th className="px-8 py-5 text-xs font-bold uppercase tracking-wider">Description</th>
                    <th className="px-8 py-5 text-xs font-bold uppercase tracking-wider text-center">Status</th>
                    <th className="px-8 py-5 text-xs font-bold uppercase tracking-wider text-right">Actions</th>
                  </tr>
                </thead>
                <tbody className={`divide-y ${isDark ? 'divide-gray-800' : 'divide-gray-100'}`}>
                  {isLoading && categories.length === 0 ? (
                    <tr>
                      <td colSpan={4} className="px-8 py-20 text-center">
                        <div className="flex flex-col items-center gap-4">
                          <div className="h-10 w-10 border-4 border-blue-600/30 border-t-blue-600 rounded-full animate-spin" />
                          <p className={`text-lg font-medium ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
                            Loading categories...
                          </p>
                        </div>
                      </td>
                    </tr>
                  ) : categories.length === 0 ? (
                    <tr>
                      <td colSpan={4} className="px-8 py-20 text-center">
                        <div className="flex flex-col items-center gap-4 text-gray-400">
                          <div className="p-6 bg-gray-500/5 rounded-full">
                            <Filter size={48} />
                          </div>
                          <p className="text-xl font-medium">No categories found</p>
                          <p className="text-sm">Try adjusting your search or add a new category.</p>
                        </div>
                      </td>
                    </tr>
                  ) : (
                    currentCategories.map((category) => (
                      <tr
                        key={category.categoryId}
                        className={`group transition-all duration-200 ${isDark ? 'hover:bg-gray-800/30' : 'hover:bg-blue-50/30'}`}
                      >
                        <td className="px-8 py-5">
                          <div className="flex items-center gap-4">
                            <div className={`h-12 w-12 rounded-2xl flex items-center justify-center font-bold text-lg select-none transition-transform group-hover:scale-110 ${isDark ? 'bg-gray-800 text-blue-400' : 'bg-blue-50 text-blue-600 border border-blue-100'
                              }`}>
                              {category.name.charAt(0).toUpperCase()}
                            </div>
                            <div>
                              <div className={`font-bold ${isDark ? 'text-white' : 'text-gray-900'}`}>{category.name}</div>
                              <div className={`text-xs mt-0.5 ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>ID: CAT-{category.categoryId}</div>
                            </div>
                          </div>
                        </td>
                        <td className="px-8 py-5">
                          <div className={`text-sm line-clamp-2 max-w-xs ${isDark ? 'text-gray-400' : 'text-gray-600'}`}>
                            {category.description || <span className="italic opacity-50">No description provided</span>}
                          </div>
                        </td>
                        <td className="px-8 py-5">
                          <div className="flex justify-center">
                            <button
                              onClick={() => toggleStatus(category.categoryId)}
                              className={`flex items-center gap-2 px-3 py-1.5 rounded-full text-xs font-bold transition-all duration-300 ${category.isActive
                                ? 'bg-green-500/10 text-green-500 hover:bg-green-500/20'
                                : 'bg-gray-500/10 text-gray-500 hover:bg-gray-500/20'
                                }`}
                            >
                              {category.isActive ? (
                                <><CheckCircle2 size={14} /> Active</>
                              ) : (
                                <><XCircle size={14} /> Inactive</>
                              )}
                            </button>
                          </div>
                        </td>
                        <td className="px-8 py-5">
                          <div className="flex items-center justify-end gap-2">
                            <button
                              onClick={() => handleEdit(category)}
                              className={`p-2.5 rounded-xl transition-all duration-200 ${isDark ? 'bg-gray-800 text-blue-400 hover:text-white hover:bg-blue-600' : 'bg-blue-50 text-blue-600 hover:text-white hover:bg-blue-600'
                                }`}
                              title="Edit Category"
                            >
                              <Edit2 size={18} />
                            </button>
                            <button
                              onClick={() => handleDelete(category.categoryId, category.name)}
                              className={`p-2.5 rounded-xl transition-all duration-200 ${isDark ? 'bg-gray-800 text-red-400 hover:text-white hover:bg-red-600' : 'bg-red-50 text-red-600 hover:text-white hover:bg-red-600'
                                }`}
                              title="Delete Category"
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

            {/* Pagination Section */}
            {totalPages > 1 && (
              <div className={`px-8 py-6 border-t flex flex-col sm:flex-row items-center justify-between gap-4 ${isDark ? 'border-gray-800 bg-gray-900/50' : 'border-gray-100 bg-gray-50/30'}`}>
                <div className={`text-sm font-medium ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
                  Showing <span className={isDark ? 'text-white' : 'text-gray-900'}>{startIndex + 1}</span> to <span className={isDark ? 'text-white' : 'text-gray-900'}>{Math.min(startIndex + itemsPerPage, categories.length)}</span> of <span className={isDark ? 'text-white' : 'text-gray-900'}>{categories.length}</span> categories
                </div>

                <div className="flex items-center gap-2">
                  <button
                    onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                    disabled={currentPage === 1}
                    className={`p-2.5 rounded-xl transition-all duration-200 flex items-center justify-center disabled:opacity-30 disabled:cursor-not-allowed ${isDark ? 'bg-gray-800 text-white hover:bg-gray-750' : 'bg-white border border-gray-200 text-gray-700 hover:bg-gray-50'
                      }`}
                  >
                    <ChevronLeft size={20} />
                  </button>

                  <div className="flex items-center gap-1.5 px-2">
                    {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                      <button
                        key={page}
                        onClick={() => setCurrentPage(page)}
                        className={`w-10 h-10 rounded-xl font-bold text-sm transition-all duration-200 ${currentPage === page
                          ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/30 ring-2 ring-blue-600 ring-offset-2 ring-offset-transparent'
                          : isDark ? 'bg-gray-800 text-gray-400 hover:text-white hover:bg-gray-700' : 'bg-white border border-gray-200 text-gray-500 hover:bg-gray-50'
                          }`}
                      >
                        {page}
                      </button>
                    ))}
                  </div>

                  <button
                    onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                    disabled={currentPage === totalPages}
                    className={`p-2.5 rounded-xl transition-all duration-200 flex items-center justify-center disabled:opacity-30 disabled:cursor-not-allowed ${isDark ? 'bg-gray-800 text-white hover:bg-gray-750' : 'bg-white border border-gray-200 text-gray-700 hover:bg-gray-50'
                      }`}
                  >
                    <ChevronRight size={20} />
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Modal */}
        <CategoryModal
          isOpen={isModalOpen}
          onClose={() => {
            setIsModalOpen(false);
            setSelectedCategory(null);
          }}
          category={selectedCategory}
        />
      </div>
    </MainLayout>
  );
}