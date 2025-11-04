import { create } from "zustand";
import { CategoryDTO } from "../types/category.js";
import { categoryApi } from "@/api/categories.js";

interface CategoryState {
  categories: CategoryDTO[];
  loading: boolean;
  fetchCategories: () => Promise<void>;
}

export const useCategoryStore = create<CategoryState>((set) => ({
  categories: [],
  loading: false,
  fetchCategories: async () => {
    set({ loading: true });
    try {
      const res = await categoryApi.getAll();
      set({ categories: res, loading: false });
    } catch (err) {
      console.error(err);
      set({ loading: false });
    }
  },
}));
