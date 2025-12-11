// stores/useCategoryStore.ts
import { create } from "zustand";
import { CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO } from "@/types/category.js";
import { categoryService } from "@/services/categoryService.js";
import { useToastStore } from "@/store/useToastStore.js";

interface CategoryState {
  // State
  categories: CategoryDTO[];
  selectedCategory: CategoryDTO | null;
  isLoading: boolean;
  error: string | null;
  
  // Actions grouped together
  actions: {
    fetchCategories: () => Promise<void>;
    createCategory: (payload: CreateCategoryDTO) => Promise<void>;
    updateCategory: (id: number, payload: UpdateCategoryDTO) => Promise<void>;
    deleteCategory: (id: number) => Promise<void>;
    selectCategory: (category: CategoryDTO | null) => void;
    clearError: () => void;
  };
}

export const useCategoryStore = create<CategoryState>((set, get) => ({
  // Initial state
  categories: [],
  selectedCategory: null,
  isLoading: false,
  error: null,

  // Actions
  actions: {
    fetchCategories: async () => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });
      try {
        const categories = await categoryService.getCategories();
        set({ categories, isLoading: false });
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to fetch categories";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },

    createCategory: async (payload: CreateCategoryDTO) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const newCategory = await categoryService.createCategory(payload);
        set((state) => ({ 
          categories: [...state.categories, newCategory] 
        }));
        toast("Category created successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to create category";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    updateCategory: async (id: number, payload: UpdateCategoryDTO) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const updatedCategory = await categoryService.updateCategory(id, payload);
        set((state) => ({
          categories: state.categories.map((cat) =>
            cat.categoryId === id ? updatedCategory : cat
          ),
        }));
        toast("Category updated successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to update category";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    deleteCategory: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        await categoryService.deleteCategory(id);
        set((state) => ({
          categories: state.categories.filter((cat) => cat.categoryId !== id),
          selectedCategory: state.selectedCategory?.categoryId === id ? null : state.selectedCategory,
        }));
        toast("Category deleted successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to delete category";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    selectCategory: (category: CategoryDTO | null) => {
      set({ selectedCategory: category });
    },

    clearError: () => {
      set({ error: null });
    },
  },
}));

// Selector hooks for optimal performance
export const useCategories = () => useCategoryStore((state) => state.categories);
export const useCategoriesLoading = () => useCategoryStore((state) => state.isLoading);
export const useCategoriesError = () => useCategoryStore((state) => state.error);
export const useSelectedCategory = () => useCategoryStore((state) => state.selectedCategory);
export const useCategoryActions = () => useCategoryStore((state) => state.actions);