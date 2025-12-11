// stores/useCustomerStore.ts
import { create } from "zustand";
import {
  CustomerDTO,
  CreateCustomerDto,
  UpdateCustomerDto,
} from "@/types/customer.js";
import {customerApi} from "@/api/customerApi.js"
import { useToastStore } from "@/store/useToastStore.js";

interface CustomerState {
  // State
  customers: CustomerDTO[];
  selectedCustomer: CustomerDTO | null;
  isLoading: boolean;
  error: string | null;

  // Actions (grouped)
  actions: {
    fetchCustomers: () => Promise<void>;
    createCustomer: (dto: CreateCustomerDto) => Promise<void>;
    updateCustomer: (id: number, dto: UpdateCustomerDto) => Promise<void>;
    deleteCustomer: (id: number) => Promise<void>;
    getCustomer: (id: number) => Promise<CustomerDTO | null>;
    selectCustomer: (customer: CustomerDTO | null) => void;
    clearError: () => void;
  };
}

export const useCustomerStore = create<CustomerState>((set, get) => ({
  // Initial state
  customers: [],
  selectedCustomer: null,
  isLoading: false,
  error: null,

  actions: {
    fetchCustomers: async () => {
      const toast = useToastStore.getState().push;
      set({ isLoading: true, error: null });

      try {
        const data = await customerApi.getAll(); // or api.getCustomers()
        set({ customers: data, isLoading: false });
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to fetch customers";
        set({ error: message, isLoading: false });
        toast(message, "error");
      }
    },

    createCustomer: async (dto: CreateCustomerDto) => {
      const toast = useToastStore.getState().push;
      set({ error: null });

      try {
        const created = await customerApi.create(dto); // or api.createCustomer(dto)
        set((state) => ({
          customers: [created, ...state.customers],
        }));
        toast("Customer created successfully", "success");
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to create customer";
        set({ error: message });
        toast(message, "error");
        throw error;
      }
    },

    updateCustomer: async (id: number, dto: UpdateCustomerDto) => {
      const toast = useToastStore.getState().push;
      set({ error: null });

      try {
        const updated = await customerApi.update(id, dto); // or api.updateCustomer(id, dto)

        // Patch local state instead of refetching
        set((state) => ({
          customers: state.customers.map((c) =>
            c.customerId === id ? updated : c
          ),
          selectedCustomer:
            state.selectedCustomer?.customerId === id
              ? updated
              : state.selectedCustomer,
        }));

        toast("Customer updated successfully", "success");
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to update customer";
        set({ error: message });
        toast(message, "error");
        throw error;
      }
    },

    deleteCustomer: async (id: number) => {
      const toast = useToastStore.getState().push;
      set({ error: null });

      try {
        await customerApi.delete(id); // or api.deleteCustomer(id)
        set((state) => ({
          customers: state.customers.filter(
            (c) => c.customerId !== id
          ),
          selectedCustomer:
            state.selectedCustomer?.customerId === id
              ? null
              : state.selectedCustomer,
        }));
        toast("Customer deleted successfully", "success");
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to delete customer";
        set({ error: message });
        toast(message, "error");
        throw error;
      }
    },

    getCustomer: async (id: number): Promise<CustomerDTO | null> => {
      const toast = useToastStore.getState().push;
      set({ error: null });

      try {
        const customer = await customerApi.getById(id); // or api.getCustomerById(id)
        return customer;
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to fetch customer";
        set({ error: message });
        toast(message, "error");
        return null;
      }
    },

    selectCustomer: (customer: CustomerDTO | null) => {
      set({ selectedCustomer: customer });
    },

    clearError: () => {
      set({ error: null });
    },
  },
}));

// Selector hooks (like product store)
export const useCustomers = () =>
  useCustomerStore((state) => state.customers);
export const useSelectedCustomer = () =>
  useCustomerStore((state) => state.selectedCustomer);
export const useCustomersLoading = () =>
  useCustomerStore((state) => state.isLoading);
export const useCustomersError = () =>
  useCustomerStore((state) => state.error);
export const useCustomerActions = () =>
  useCustomerStore((state) => state.actions);
