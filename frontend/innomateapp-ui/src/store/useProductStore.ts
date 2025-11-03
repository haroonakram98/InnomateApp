import { create } from "zustand";
import { ProductDTO, CreateProductDto } from "../types/product.js";
import { productApi } from "../api/products.js";

interface ProductState {
  products: ProductDTO[];
  loading: boolean;
  error: string | null;

  // Actions
  fetchProducts: () => Promise<void>;
  createProduct: (data: CreateProductDto) => Promise<void>;
  updateProduct: (id: number, data: ProductDTO) => Promise<void>;
  deleteProduct: (id: number) => Promise<void>;
  setProducts: (products: ProductDTO[]) => void;
}

export const useProductStore = create<ProductState>((set, get) => ({
  products: [],
  loading: false,
  error: null,

  setProducts: (products) => set({ products }),

  fetchProducts: async () => {
    set({ loading: true, error: null });
    try {
      const data = await productApi.getAll();
      set({ products: data, loading: false });
    } catch (error: any) {
      console.error("Error fetching products:", error);
      set({
        loading: false,
        error: error.message || "Failed to load products",
      });
    }
  },

  createProduct: async (data: CreateProductDto) => {
    try {
      const newProduct = await productApi.create(data);
      if (newProduct) {
        set({ products: [...get().products, newProduct] });
      }
    } catch (error: any) {
      console.error("Error creating product:", error);
      set({ error: error.message || "Failed to create product" });
      throw error;
    }
  },

  updateProduct: async (id: number, data: ProductDTO) => {
    try {
      const updated = await productApi.update(id, data);
      if (updated) {
        set({
          products: get().products.map((p) =>
            p.productId === id ? updated : p
          ),
        });
      }
    } catch (error: any) {
      console.error("Error updating product:", error);
      set({ error: error.message || "Failed to update product" });
      throw error;
    }
  },

  deleteProduct: async (id: number) => {
    try {
      await productApi.delete(id);
      set({ products: get().products.filter((p) => p.productId !== id) });
    } catch (error: any) {
      console.error("Error deleting product:", error);
      set({ error: error.message || "Failed to delete product" });
      throw error;
    }
  },
}));
