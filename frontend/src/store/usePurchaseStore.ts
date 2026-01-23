import { create } from "zustand";
import { PurchaseDTO, CreatePurchaseDTO } from "@/types/purchase.js";
import { purchaseService } from "@/services/purchaseService.js";
import { useToastStore } from "@/store/useToastStore.js";

interface PurchaseState {
  purchases: PurchaseDTO[];
  selectedPurchase: PurchaseDTO | null;
  isLoading: boolean;
  error: string | null;

  actions: {
    fetchPurchases: (startDate?: string, endDate?: string, search?: string) => Promise<void>;
    fetchPurchaseById: (id: number) => Promise<void>;
    createPurchase: (payload: CreatePurchaseDTO) => Promise<void>;
    receivePurchase: (id: number) => Promise<void>;
    cancelPurchase: (id: number) => Promise<void>;
    getNextPurchaseNumber: () => Promise<string | null>;
    getNextBatchNumber: () => Promise<string | null>;
    selectPurchase: (purchase: PurchaseDTO | null) => void;
    clearError: () => void;
  };
}

export const usePurchaseStore = create<PurchaseState>((set, get) => ({
  purchases: [],
  selectedPurchase: null,
  isLoading: false,
  error: null,

  actions: {
    fetchPurchases: async (startDate?: string, endDate?: string, search?: string) => {
      set({ isLoading: true, error: null });
      try {
        const purchases = await purchaseService.getPurchasesByDateRange(
          startDate || new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString(),
          endDate || new Date().toISOString(),
          search
        );
        set({ purchases, isLoading: false });
      } catch (error: any) {
        const message = error.response?.data || "Failed to fetch purchases";
        set({ error: message, isLoading: false });
      }
    },

    fetchPurchaseById: async (id: number) => {
      set({ isLoading: true, error: null });
      try {
        const purchase = await purchaseService.getPurchaseById(id);
        set({ selectedPurchase: purchase, isLoading: false });
      } catch (error: any) {
        const message = error.response?.data || "Failed to fetch purchase details";
        set({ error: message, isLoading: false });
      }
    },

    createPurchase: async (payload: CreatePurchaseDTO) => {
      const toast = useToastStore.getState().push;
      try {
        await purchaseService.createPurchase(payload);
        await get().actions.fetchPurchases();
        toast("Purchase record created successfully", "success");
      } catch (error: any) {
        const message = error.response?.data || "Failed to create purchase";
        toast(message, "error");
        throw error;
      }
    },

    receivePurchase: async (id: number) => {
      const toast = useToastStore.getState().push;
      try {
        const updated = await purchaseService.receivePurchase(id);
        set(state => ({
          purchases: state.purchases.map(p => p.purchaseId === id ? updated : p),
          selectedPurchase: state.selectedPurchase?.purchaseId === id ? updated : state.selectedPurchase
        }));
        toast("Inventory updated: Purchase received", "success");
      } catch (error: any) {
        const message = error.response?.data || "Failed to receive purchase";
        toast(message, "error");
      }
    },

    cancelPurchase: async (id: number) => {
      const toast = useToastStore.getState().push;
      try {
        await purchaseService.cancelPurchase(id);
        set(state => ({
          purchases: state.purchases.map(p => p.purchaseId === id ? { ...p, status: 'Cancelled' } : p),
          selectedPurchase: state.selectedPurchase?.purchaseId === id ? { ...state.selectedPurchase, status: 'Cancelled' } : state.selectedPurchase
        }));
        toast("Purchase order cancelled", "info");
      } catch (error: any) {
        const message = error.response?.data || "Failed to cancel purchase";
        toast(message, "error");
      }
    },

    getNextPurchaseNumber: async () => {
      try {
        return await purchaseService.getNextPurchaseNumber();
      } catch (error) {
        return null;
      }
    },

    getNextBatchNumber: async () => {
      try {
        return await purchaseService.getNextBatchNumber();
      } catch (error) {
        return null;
      }
    },

    selectPurchase: (purchase) => set({ selectedPurchase: purchase }),
    clearError: () => set({ error: null }),
  },
}));

export const usePurchases = () => usePurchaseStore((state) => state.purchases);
export const usePurchasesLoading = () => usePurchaseStore((state) => state.isLoading);
export const usePurchasesError = () => usePurchaseStore((state) => state.error);
export const useSelectedPurchase = () => usePurchaseStore((state) => state.selectedPurchase);
export const usePurchaseActions = () => usePurchaseStore((state) => state.actions);