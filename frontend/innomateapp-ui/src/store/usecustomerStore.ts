import { create } from "zustand";
import { Customer, CreateCustomerDto, UpdateCustomerDto } from "@/types/customer.js";
import * as api from "@/api/customerApi.js";

interface CustomerState {
  customers: Customer[];
  loading: boolean;
  fetchCustomers: () => Promise<void>;
  createCustomer: (dto: CreateCustomerDto) => Promise<void>;
  updateCustomer: (id: number, dto: UpdateCustomerDto) => Promise<void>;
  deleteCustomer: (id: number) => Promise<void>;
  getCustomer: (id: number) => Promise<Customer | null>;
}

export const useCustomerStore = create<CustomerState>((set, get) => ({
  customers: [],
  loading: false,

  fetchCustomers: async () => {
    set({ loading: true });
    try {
      const data = await api.getCustomers();
      set({ customers: data });
    } finally {
      set({ loading: false });
    }
  },

  createCustomer: async (dto) => {
    set({ loading: true });
    try {
      const created = await api.createCustomer(dto);
      // append created customer
      set((s) => ({ customers: [created, ...s.customers] }));
    } finally {
      set({ loading: false });
    }
  },

  updateCustomer: async (id, dto) => {
    set({ loading: true });
    try {
      await api.updateCustomer(id, dto);
      // reload list or patch local state
      await get().fetchCustomers();
    } finally {
      set({ loading: false });
    }
  },

  deleteCustomer: async (id) => {
    set({ loading: true });
    try {
      await api.deleteCustomer(id);
      set((s) => ({ customers: s.customers.filter((c) => c.customerId !== id) }));
    } finally {
      set({ loading: false });
    }
  },

  getCustomer: async (id) => {
    try {
      const customer = await api.getCustomerById(id);
      return customer;
    } catch {
      return null;
    }
  },
}));
