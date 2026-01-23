import { create } from "zustand";
import { CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO } from "@/types/category.js";
import { categoryApi } from "@/api/categories.js";
import { useToastStore } from "@/store/useToastStore.js";

interface CategoryState {
  categories: CategoryDTO[];
  selectedCategory: CategoryDTO | null;
  isLoading: boolean;
  error: string | null;
  validationErrors: Record<string, string[]> | null;

  actions: {
    fetchCategories: (search?: string) => Promise<void>;
    createCategory: (payload: CreateCategoryDTO) => Promise<void>;
    updateCategory: (id: number, payload: UpdateCategoryDTO) => Promise<void>;
    deleteCategory: (id: number) => Promise<void>;
    toggleStatus: (id: number) => Promise<void>;
    setSelectedCategory: (category: CategoryDTO | null) => void;
    clearError: () => void;
  };
}

export const useCategoryStore = create<CategoryState>((set, get) => ({
  categories: [],
  selectedCategory: null,
  isLoading: false,
  error: null,
  validationErrors: null,

  actions: {
    fetchCategories: async (search?: string) => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null, validationErrors: null });
      try {
        const categories = await categoryApi.getAll(search);
        set({ categories, isLoading: false });
      } catch (error: any) {
        const message = error.response?.data?.detail || "Failed to fetch categories";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },

    createCategory: async (payload: CreateCategoryDTO) => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null, validationErrors: null });
      try {
        const newCategory = await categoryApi.create(payload);
        set((state) => ({
          categories: [newCategory, ...state.categories],
          isLoading: false
        }));
        toast("Category created successfully", 'success');
      } catch (error: any) {
        const status = error.response?.status;
        const data = error.response?.data;

        if (status === 400 && data?.errors) {
          set({ validationErrors: data.errors, isLoading: false });
        } else {
          const message = data?.detail || "Failed to create category";
          set({ error: message, isLoading: false });
          toast(message, 'error');
        }
        throw error;
      }
    },

    updateCategory: async (id: number, payload: UpdateCategoryDTO) => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null, validationErrors: null });
      try {
        const updatedCategory = await categoryApi.update(id, payload);
        set((state) => ({
          categories: state.categories.map((cat) =>
            cat.categoryId === id ? updatedCategory : cat
          ),
          isLoading: false
        }));
        toast("Category updated successfully", 'success');
      } catch (error: any) {
        const status = error.response?.status;
        const data = error.response?.data;

        if (status === 400 && data?.errors) {
          set({ validationErrors: data.errors, isLoading: false });
        } else {
          const message = data?.detail || "Failed to update category";
          set({ error: message, isLoading: false });
          toast(message, 'error');
        }
        throw error;
      }
    },

    deleteCategory: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });
      try {
        await categoryApi.delete(id);
        set((state) => ({
          categories: state.categories.filter((cat) => cat.categoryId !== id),
          selectedCategory: state.selectedCategory?.categoryId === id ? null : state.selectedCategory,
          isLoading: false
        }));
        toast("Category deleted successfully", 'success');
      } catch (error: any) {
        const message = error.response?.data?.detail || "Failed to delete category";
        set({ error: message, isLoading: false });
        toast(message, 'error');
        throw error;
      }
    },

    toggleStatus: async (id: number) => {
      const toast = useToastStore.getState().push;
      try {
        const { isActive } = await categoryApi.toggleStatus(id);
        set((state) => ({
          categories: state.categories.map((cat) =>
            cat.categoryId === id ? { ...cat, isActive } : cat
          ),
        }));
        toast(`Category ${isActive ? 'activated' : 'deactivated'} successfully`, 'success');
      } catch (error: any) {
        const message = error.response?.data?.detail || "Failed to toggle status";
        toast(message, 'error');
      }
    },

    setSelectedCategory: (category: CategoryDTO | null) => {
      set({ selectedCategory: category });
    },

    clearError: () => {
      set({ error: null, validationErrors: null });
    },
  }
}));

// Selector hooks for compatibility and performance
export const useCategories = () => useCategoryStore((state) => state.categories);
export const useSelectedCategory = () => useCategoryStore((state) => state.selectedCategory);
export const useCategoriesLoading = () => useCategoryStore((state) => state.isLoading);
export const useCategoriesError = () => useCategoryStore((state) => state.error);
export const useCategoryValidationErrors = () => useCategoryStore((state) => state.validationErrors);
export const useCategoryActions = () => useCategoryStore((state) => state.actions);