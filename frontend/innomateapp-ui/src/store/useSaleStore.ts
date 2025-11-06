// src/store/useSaleStore.ts
import { create } from "zustand";
import type { SaleDTO } from "@/types/sale.js";
import * as api from "@/api/saleApi.js";

type State = {
  sales: SaleDTO[];
  loading: boolean;
  fetchSales: () => Promise<void>;
  createSale: (s: SaleDTO) => Promise<SaleDTO | null>;
  updateSale: (id: number, s: SaleDTO) => Promise<SaleDTO | null>;
  deleteSale: (id: number) => Promise<boolean>;
  getById: (id: number) => Promise<SaleDTO | null>;
};

export const useSaleStore = create<State>((set, get) => ({
  sales: [],
  loading: false,

  fetchSales: async () => {
    set({ loading: true });
    try {
      const data = await api.getSales();
      set({ sales: data });
    } finally {
      set({ loading: false });
    }
  },

  createSale: async (dto) => {
    set({ loading: true });
    try {
      const created = await api.createSale(dto);
      set((s) => ({ sales: [created, ...s.sales] }));
      return created;
    } catch (err) {
      console.error(err);
      return null;
    } finally {
      set({ loading: false });
    }
  },

  updateSale: async (id, dto) => {
    set({ loading: true });
    try {
      const updated = await api.updateSale(id, dto);
      set((s) => ({ sales: s.sales.map(x => x.saleId === id ? updated : x) }));
      return updated;
    } catch (err) {
      console.error(err);
      return null;
    } finally {
      set({ loading: false });
    }
  },

  deleteSale: async (id) => {
    set({ loading: true });
    try {
      await api.deleteSale(id);
      set((s) => ({ sales: s.sales.filter(x => x.saleId !== id) }));
      return true;
    } catch (err) {
      console.error(err);
      return false;
    } finally {
      set({ loading: false });
    }
  },

  getById: async (id) => {
    try {
      const s = await api.getSaleById(id);
      return s;
    } catch {
      return null;
    }
  }
}));
