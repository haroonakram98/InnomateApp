// src/store/usePurchaseStore.ts
import { create } from "zustand";
import { PurchaseDTO, CreatePurchaseDTO, UpdatePurchaseDTO } from "@/types/purchase.js";
import { purchaseService } from "@/services/purchaseService.js";
import { useToastStore } from "@/store/useToastStore.js";

interface PurchaseState {
  // State
  purchases: PurchaseDTO[];
  selectedPurchase: PurchaseDTO | null;
  isLoading: boolean;
  error: string | null;

  // Actions
  actions: {
    fetchPurchases: () => Promise<void>;
    fetchPurchasesByDateRange: (startDate: string, endDate: string) => Promise<void>;
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
  // Initial state
  purchases: [],
  selectedPurchase: null,
  isLoading: false,
  error: null,

  // Actions
  actions: {
    fetchPurchases: async () => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });
      try {
        const purchases = await purchaseService.getPurchases();
        set({ purchases, isLoading: false });
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to fetch purchases";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },

    fetchPurchasesByDateRange: async (startDate: string, endDate: string) => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });
      try {
        const purchases = await purchaseService.getPurchasesByDateRange(startDate, endDate);
        set({ purchases, isLoading: false });
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to fetch purchases";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },

    fetchPurchaseById: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const purchase = await purchaseService.getPurchaseById(id);
        set({ selectedPurchase: purchase });
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to fetch purchase";
        set({ error: message });
        toast(message, 'error');
      }
    },

    createPurchase: async (payload: CreatePurchaseDTO) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const newPurchase = await purchaseService.createPurchase(payload);
        set((state) => ({
          purchases: [...state.purchases, newPurchase]
        }));
        toast("Purchase created successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to create purchase";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    receivePurchase: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        const updatedPurchase = await purchaseService.receivePurchase(id);
        set((state) => ({
          purchases: state.purchases.map((purchase) =>
            purchase.purchaseId === id ? updatedPurchase : purchase
          ),
        }));
        toast("Purchase received successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to receive purchase";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    cancelPurchase: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null });
      try {
        await purchaseService.cancelPurchase(id);
        set((state) => ({
          purchases: state.purchases.map((purchase) =>
            purchase.purchaseId === id
              ? { ...purchase, status: 'Cancelled' }
              : purchase
          ),
        }));
        toast("Purchase cancelled successfully", 'success');
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to cancel purchase";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    getNextPurchaseNumber: async () => {
      try {
        return await purchaseService.getNextPurchaseNumber();
      } catch (error) {
        console.error("Failed to fetch next purchase number:", error);
        return null;
      }
    },

    getNextBatchNumber: async () => {
      try {
        return await purchaseService.getNextBatchNumber();
      } catch (error) {
        console.error("Failed to fetch next batch number:", error);
        return null;
      }
    },

    selectPurchase: (purchase: PurchaseDTO | null) => {
      set({ selectedPurchase: purchase });
    },

    clearError: () => {
      set({ error: null });
    },
  },
}));

// Selector hooks for optimal performance
export const usePurchases = () => usePurchaseStore((state) => state.purchases);
export const usePurchasesLoading = () => usePurchaseStore((state) => state.isLoading);
export const usePurchasesError = () => usePurchaseStore((state) => state.error);
export const useSelectedPurchase = () => usePurchaseStore((state) => state.selectedPurchase);
export const usePurchaseActions = () => usePurchaseStore((state) => state.actions);