import { create } from "zustand";
import { ProductDTO, CreateProductDto, UpdateProductDto, ProductStockDto } from "@/types/product.js";
import { productService } from "@/services/productService.js";
import { useToastStore } from "@/store/useToastStore.js";

interface ProductState {
  products: ProductDTO[];
  salesProducts: ProductStockDto[];
  selectedProduct: ProductDTO | null;
  isLoading: boolean;
  error: string | null;

  actions: {
    fetchProducts: (search?: string) => Promise<void>;
    fetchProductsForSales: () => Promise<void>;
    createProduct: (payload: CreateProductDto) => Promise<void>;
    updateProduct: (payload: UpdateProductDto) => Promise<void>;
    deleteProduct: (id: number) => Promise<void>;
    selectProduct: (product: ProductDTO | null) => void;
    clearError: () => void;
  };
}

export const useProductStore = create<ProductState>((set, get) => ({
  products: [],
  salesProducts: [],
  selectedProduct: null,
  isLoading: false,
  error: null,

  actions: {
    fetchProducts: async (search?: string) => {
      set({ isLoading: true, error: null });
      try {
        const products = await productService.getProducts();
        // Filtering in memory if search present, or you could update API to support search
        let filtered = products;
        if (search) {
          const s = search.toLowerCase();
          filtered = products.filter(p => p.name.toLowerCase().includes(s) || (p.SKU && p.SKU.toLowerCase().includes(s)));
        }
        set({ products: filtered, isLoading: false });
      } catch (error: any) {
        const message = error.response?.data || "Failed to fetch products";
        set({ error: message, isLoading: false });
      }
    },

    fetchProductsForSales: async () => {
      set({ isLoading: true, error: null });
      try {
        const products = await productService.fetchProductsForSale();
        set({ salesProducts: products, isLoading: false });
      } catch (error: any) {
        const message = error.response?.data || "Failed to fetch sales products";
        set({ error: message, isLoading: false });
      }
    },

    createProduct: async (payload: CreateProductDto) => {
      const toast = useToastStore.getState().push;
      try {
        await productService.createProduct(payload);
        await get().actions.fetchProducts();
        toast("Product catalog updated: Item created", "success");
      } catch (error: any) {
        const message = error.response?.data || "Failed to create product";
        toast(message, "error");
        throw error;
      }
    },

    updateProduct: async (payload: UpdateProductDto) => {
      const toast = useToastStore.getState().push;
      try {
        const updated = await productService.updateProduct(payload);
        set(state => ({
          products: state.products.map(p => p.productId === payload.productId ? updated : p),
          selectedProduct: state.selectedProduct?.productId === payload.productId ? updated : state.selectedProduct
        }));
        toast("Product specifications updated", "success");
      } catch (error: any) {
        const message = error.response?.data || "Failed to update product";
        toast(message, "error");
        throw error;
      }
    },

    deleteProduct: async (id: number) => {
      const toast = useToastStore.getState().push;
      try {
        await productService.deleteProduct(id);
        set(state => ({
          products: state.products.filter(p => p.productId !== id),
          selectedProduct: state.selectedProduct?.productId === id ? null : state.selectedProduct
        }));
        toast("Product retired from catalog", "info");
      } catch (error: any) {
        const message = error.response?.data || "Failed to delete product";
        toast(message, "error");
      }
    },

    selectProduct: (product) => set({ selectedProduct: product }),
    clearError: () => set({ error: null }),
  },
}));

export const useProducts = () => useProductStore((state) => state.products);
export const useSalesProducts = () => useProductStore((state) => state.salesProducts);
export const useProductsLoading = () => useProductStore((state) => state.isLoading);
export const useProductsError = () => useProductStore((state) => state.error);
export const useSelectedProduct = () => useProductStore((state) => state.selectedProduct);
export const useProductActions = () => useProductStore((state) => state.actions);