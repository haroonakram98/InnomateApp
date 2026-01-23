// src/store/useSupplierStore.ts
import { create } from "zustand";
import { SupplierDTO, CreateSupplierDTO, UpdateSupplierDTO, SupplierDetailDto, SupplierStats } from "@/types/supplier.js";
import { supplierService } from "@/api/supplierService.js";
import { useToastStore } from "@/store/useToastStore.js";

interface SupplierState {
  // State
  suppliers: SupplierDTO[];
  selectedSupplier: SupplierDTO | null;
  supplierDetail: SupplierDetailDto | null;
  supplierStats: SupplierStats | null;
  isLoading: boolean;
  error: string | null;
  validationErrors: Record<string, string[]> | null;

  // Actions
  actions: {
    fetchSuppliers: (search?: string) => Promise<void>;
    fetchSupplierById: (id: number) => Promise<void>;
    fetchSupplierDetail: (id: number) => Promise<void>;
    fetchSupplierStats: (id: number) => Promise<void>;
    createSupplier: (payload: CreateSupplierDTO) => Promise<void>;
    updateSupplier: (id: number, payload: UpdateSupplierDTO) => Promise<void>;
    deleteSupplier: (id: number) => Promise<void>;
    toggleSupplierStatus: (id: number) => Promise<void>;
    selectSupplier: (supplier: SupplierDTO | null) => void;
    setValidationErrors: (errors: Record<string, string[]> | null) => void;
    clearError: () => void;
  };
}

export const useSupplierStore = create<SupplierState>((set, get) => ({
  // Initial state
  suppliers: [],
  selectedSupplier: null,
  supplierDetail: null,
  supplierStats: null,
  isLoading: false,
  error: null,
  validationErrors: null,

  // Actions
  actions: {
    fetchSuppliers: async (search?: string) => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null, validationErrors: null });
      try {
        const suppliers = await supplierService.getSuppliers(search);
        set({ suppliers, isLoading: false });
      } catch (error) {
        const message = (error as any).message || "Failed to fetch suppliers";
        set({ error: message, isLoading: false });
        toast(message, 'error');
      }
    },

    fetchSupplierById: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        const supplier = await supplierService.getSupplierById(id);
        set({ selectedSupplier: supplier });
      } catch (error) {
        const message = (error as any).message || "Failed to fetch supplier";
        set({ error: message });
        toast(message, 'error');
      }
    },

    fetchSupplierDetail: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        const supplierDetail = await supplierService.getSupplierWithPurchases(id);
        set({ supplierDetail });
      } catch (error) {
        const message = (error as any).message || "Failed to fetch supplier details";
        set({ error: message });
        toast(message, 'error');
      }
    },

    fetchSupplierStats: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        const stats = await supplierService.getSupplierStats(id);
        set({ supplierStats: stats });
      } catch (error) {
        const message = (error as any).message || "Failed to fetch supplier stats";
        set({ error: message });
        toast(message, 'error');
      }
    },

    createSupplier: async (payload: CreateSupplierDTO) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        const newSupplier = await supplierService.createSupplier(payload);
        set((state) => ({
          suppliers: [...state.suppliers, newSupplier]
        }));
        toast("Supplier created successfully", 'success');
      } catch (error: any) {
        const message = error.message || "Failed to create supplier";
        set({
          error: message,
          validationErrors: error.errors || null
        });

        // 🤫 Only toast if it's NOT a validation error.
        if (!error.errors) {
          toast(message, 'error');
        }
        throw error;
      }
    },

    updateSupplier: async (id: number, payload: UpdateSupplierDTO) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        const updatedSupplier = await supplierService.updateSupplier(id, payload);
        set((state) => ({
          suppliers: state.suppliers.map((supplier) =>
            supplier.supplierId === id ? updatedSupplier : supplier
          ),
          selectedSupplier: state.selectedSupplier?.supplierId === id ? updatedSupplier : state.selectedSupplier,
          supplierDetail: state.supplierDetail?.supplierId === id
            ? { ...state.supplierDetail, ...updatedSupplier }
            : state.supplierDetail,
        }));
        toast("Supplier updated successfully", 'success');
      } catch (error: any) {
        const message = error.message || "Failed to update supplier";
        set({
          error: message,
          validationErrors: error.errors || null
        });

        // 🤫 No toast for validation errors
        if (!error.errors) {
          toast(message, 'error');
        }
        throw error;
      }
    },

    deleteSupplier: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        await supplierService.deleteSupplier(id);
        set((state) => ({
          suppliers: state.suppliers.filter((supplier) => supplier.supplierId !== id),
          selectedSupplier: state.selectedSupplier?.supplierId === id ? null : state.selectedSupplier,
          supplierDetail: state.supplierDetail?.supplierId === id ? null : state.supplierDetail,
        }));
        toast("Supplier deleted successfully", 'success');
      } catch (error) {
        const message = (error as any).message || "Failed to delete supplier";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    toggleSupplierStatus: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null, validationErrors: null });
      try {
        const result = await supplierService.toggleSupplierStatus(id);
        set((state) => ({
          suppliers: state.suppliers.map((supplier) =>
            supplier.supplierId === id ? { ...supplier, isActive: result.isActive } : supplier
          ),
          selectedSupplier: state.selectedSupplier?.supplierId === id
            ? { ...state.selectedSupplier, isActive: result.isActive }
            : state.selectedSupplier,
          supplierDetail: state.supplierDetail?.supplierId === id
            ? { ...state.supplierDetail, isActive: result.isActive }
            : state.supplierDetail,
        }));
        toast(`Supplier ${result.isActive ? 'activated' : 'deactivated'} successfully`, 'success');
      } catch (error) {
        const message = (error as any).message || "Failed to toggle supplier status";
        set({ error: message });
        toast(message, 'error');
        throw error;
      }
    },

    selectSupplier: (supplier: SupplierDTO | null) => {
      set({ selectedSupplier: supplier });
    },

    setValidationErrors: (errors: Record<string, string[]> | null) => {
      set({ validationErrors: errors });
    },

    clearError: () => {
      set({ error: null, validationErrors: null });
    },
  },
}));

// Selector hooks for optimal performance
export const useSuppliers = () => useSupplierStore((state) => state.suppliers);
export const useSuppliersLoading = () => useSupplierStore((state) => state.isLoading);
export const useSuppliersError = () => useSupplierStore((state) => state.error);
export const useValidationErrors = () => useSupplierStore((state) => state.validationErrors);
export const useSelectedSupplier = () => useSupplierStore((state) => state.selectedSupplier);
export const useSupplierDetail = () => useSupplierStore((state) => state.supplierDetail);
export const useSupplierStats = () => useSupplierStore((state) => state.supplierStats);
export const useSupplierActions = () => useSupplierStore((state) => state.actions);