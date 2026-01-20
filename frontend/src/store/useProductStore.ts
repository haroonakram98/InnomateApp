// stores/useProductStore.ts
import { create } from "zustand";
import { ProductDTO, CreateProductDto, UpdateProductDto, ProductStockDto, ProductLookupDTO } from "@/types/product.js";
import { productService } from "@/services/productService.js";
import { useToastStore } from "@/store/useToastStore.js";
import { SupplierDTO } from "@/types/supplier.js";

interface ProductState {
  // State
  products: ProductDTO[];
  salesProducts: ProductStockDto[];
  lookup: ProductLookupDTO[];
  selectedProduct: ProductDTO | null;
  isLoading: boolean;
  error: string | null;

  // Actions grouped together
  actions: {
    fetchProducts: () => Promise<void>;
    createProduct: (payload: CreateProductDto) => Promise<void>;
    updateProduct: (payload: UpdateProductDto) => Promise<void>;
    deleteProduct: (id: number) => Promise<void>;
    selectProduct: (product: ProductDTO | null) => void;
    clearError: () => void;
    fetchProductsForSales: () => Promise<void>;
    fetchLookup: () => Promise<void>;
  };
}

export const useProductStore = create<ProductState>((set, get) => ({
  // Initial state
  products: [],
  salesProducts: [],
  lookup: [],
  selectedProduct: null,
  isLoading: false,
  error: null,

  // Actions
  actions: {
    fetchProducts: async () => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });
      try {
        const products = await productService.getProducts();
        set({ products, isLoading: false });
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to fetch products";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },

    createProduct: async (payload: CreateProductDto) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const newProduct = await productService.createProduct(payload);
        set((state) => ({
          products: [...state.products, newProduct]
        }));
        toast("Product created successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to create product";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    updateProduct: async (payload: UpdateProductDto) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const updatedProduct = await productService.updateProduct(payload);
        set((state) => ({
          products: state.products.map((product) =>
            product.productId === payload.productId ? updatedProduct : product
          ),
        }));
        toast("Product updated successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to update product";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    deleteProduct: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        await productService.deleteProduct(id);
        set((state) => ({
          products: state.products.filter((product) => product.productId !== id),
          selectedProduct: state.selectedProduct?.productId === id ? null : state.selectedProduct,
        }));
        toast("Product deleted successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to delete product";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    selectProduct: (product: ProductDTO | null) => {
      set({ selectedProduct: product });
    },

    clearError: () => {
      set({ error: null });
    },
    fetchProductsForSales: async () => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });
      try {
        const products = await productService.fetchProductsForSale();
        set({ salesProducts: products, isLoading: false });
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to fetch products";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },
    fetchLookup: async () => {
      try {
        const lookup = await productService.getLookup();
        set({ lookup });
      } catch (error) {
        console.error("Failed to fetch product lookup", error);
      }
    }
  },
}));

// Selector hooks for optimal performance
export const useProducts = () => useProductStore((state) => state.products);
export const useSalesProducts = () => useProductStore((state) => state.salesProducts);
export const useProductLookup = () => useProductStore((state) => state.lookup);
export const useProductsLoading = () => useProductStore((state) => state.isLoading);
export const useProductsError = () => useProductStore((state) => state.error);
export const useSelectedProduct = () => useProductStore((state) => state.selectedProduct);
export const useProductActions = () => useProductStore((state) => state.actions);