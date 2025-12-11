// src/store/useSaleStore.ts
import { create } from "zustand";
import type { SaleDTO, CreateSaleDTO } from "@/types/sale.js";
import * as api from "@/api/saleApi.js";

type State = {
  sales: SaleDTO[];
  loading: boolean;
  error: string | null;
  fetchSales: () => Promise<void>;
  createSale: (s: CreateSaleDTO) => Promise<SaleDTO | null>;
  updateSale: (id: number, s: Partial<CreateSaleDTO>) => Promise<SaleDTO | null>;
  deleteSale: (id: number) => Promise<boolean>;
  getById: (id: number) => Promise<SaleDTO | null>;
  getNextInvoiceNumber: () => Promise<string | null>;
  clearError: () => void;
};

export const useSaleStore = create<State>((set, get) => ({
  sales: [],
  loading: false,
  error: null,

  fetchSales: async () => {
    set({ loading: true, error: null });
    try {
      const data = await api.getSales();
      set({ sales: data });
    } catch (err: any) {
      set({ error: err.message || "Failed to fetch sales" });
      console.error("Error fetching sales:", err);
    } finally {
      set({ loading: false });
    }
  },

  createSale: async (dto: CreateSaleDTO) => {
    set({ loading: true, error: null });
    try {
      debugger
      // Validate required fields
      // if (!dto.customerId) {
      //   throw new Error("Customer ID is required");
      // }

      if (!dto.saleDetails || dto.saleDetails.length === 0) {
        throw new Error("Sale must have at least one item");
      }

      // Calculate totals if not provided
      const saleWithTotals = {
        ...dto,
        // Ensure all required fields are present
        paidAmount: dto.paidAmount || 0,
        balanceAmount: dto.balanceAmount || 0,
        isFullyPaid: dto.isFullyPaid || false,
        discount: dto.discount || 0,
        discountPercentage: dto.discountPercentage || 0,
        discountType: dto.discountType || 'Rs',
      };

      const created = await api.createSale(saleWithTotals);

      // Add to store state
      set((state) => ({
        sales: [created, ...state.sales],
        error: null
      }));

      return created;
    } catch (err: any) {
      const errorMessage = err.message || "Failed to create sale";
      set({ error: errorMessage });
      console.error("Error creating sale:", err);
      return null;
    } finally {
      set({ loading: false });
    }
  },

  updateSale: async (id: number, dto: Partial<CreateSaleDTO>) => {
    set({ loading: true, error: null });
    try {
      const updated = await api.updateSale(id, dto);
      set((state) => ({
        sales: state.sales.map(x => x.saleId === id ? updated : x),
        error: null
      }));
      return updated;
    } catch (err: any) {
      const errorMessage = err.message || "Failed to update sale";
      set({ error: errorMessage });
      console.error("Error updating sale:", err);
      return null;
    } finally {
      set({ loading: false });
    }
  },

  deleteSale: async (id: number) => {
    set({ loading: true, error: null });
    try {
      await api.deleteSale(id);
      set((state) => ({
        sales: state.sales.filter(x => x.saleId !== id),
        error: null
      }));
      return true;
    } catch (err: any) {
      const errorMessage = err.message || "Failed to delete sale";
      set({ error: errorMessage });
      console.error("Error deleting sale:", err);
      return false;
    } finally {
      set({ loading: false });
    }
  },

  getById: async (id: number) => {
    set({ error: null });
    try {
      const sale = await api.getSaleById(id);
      return sale;
    } catch (err: any) {
      const errorMessage = err.message || "Failed to fetch sale";
      set({ error: errorMessage });
      console.error("Error fetching sale:", err);
      return null;
    }
  },

  getNextInvoiceNumber: async () => {
    try {
      return await api.getNextInvoiceNumber();
    } catch (err: any) {
      console.error("Error fetching next invoice number:", err);
      return null;
    }
  },

  clearError: () => set({ error: null }),
}));

// Selector hooks for better performance
export const useSales = () => useSaleStore(state => state.sales);
export const useSalesLoading = () => useSaleStore(state => state.loading);
export const useSalesError = () => useSaleStore(state => state.error);
export const useSaleActions = () => useSaleStore(state => ({
  fetchSales: state.fetchSales,
  createSale: state.createSale,
  updateSale: state.updateSale,
  deleteSale: state.deleteSale,
  getById: state.getById,
  getNextInvoiceNumber: state.getNextInvoiceNumber,
  clearError: state.clearError,
}));