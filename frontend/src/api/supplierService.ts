// src/services/supplierService.ts
import api from "@/lib/utils/axios.js"
import {
  SupplierDTO,
  CreateSupplierDTO,
  UpdateSupplierDTO,
  SupplierWithStatsDTO,
  SupplierDetailDto
} from '@/types/supplier.js';

export const supplierService = {
  // Get all suppliers
  getSuppliers: async (search?: string): Promise<SupplierDTO[]> => {
    const response = await api.get('/suppliers', {
      params: { search }
    });
    return response.data;
  },

  // Get active suppliers
  getActiveSuppliers: async (): Promise<SupplierDTO[]> => {
    const response = await api.get('/suppliers/active');
    return response.data;
  },

  // Get top suppliers
  getTopSuppliers: async (count: number = 10): Promise<SupplierDTO[]> => {
    const response = await api.get('/suppliers/top', {
      params: { count }
    });
    return response.data;
  },

  // Get suppliers with recent purchases
  getSuppliersWithRecentPurchases: async (days: number = 30): Promise<SupplierDTO[]> => {
    const response = await api.get('/suppliers/recent', {
      params: { days }
    });
    return response.data;
  },

  // Get supplier by ID
  getSupplierById: async (id: number): Promise<SupplierDTO> => {
    const response = await api.get(`/suppliers/${id}`);
    return response.data;
  },

  // Get supplier with purchases
  getSupplierWithPurchases: async (id: number): Promise<SupplierDetailDto> => {
    const response = await api.get(`/suppliers/${id}/with-purchases`);
    return response.data;
  },

  // Get supplier stats
  getSupplierStats: async (id: number): Promise<any> => {
    const response = await api.get(`/suppliers/${id}/stats`);
    return response.data;
  },

  // Create supplier
  createSupplier: async (payload: CreateSupplierDTO): Promise<SupplierDTO> => {
    const response = await api.post('/suppliers', payload);
    return response.data;
  },

  // Update supplier
  updateSupplier: async (id: number, payload: UpdateSupplierDTO): Promise<SupplierDTO> => {
    const response = await api.put(`/suppliers/${id}`, payload);
    return response.data;
  },

  // Delete supplier
  deleteSupplier: async (id: number): Promise<void> => {
    const response = await api.delete(`/suppliers/${id}`);
    return response.data;
  },

  // Toggle supplier status
  toggleSupplierStatus: async (id: number): Promise<{ isActive: boolean }> => {
    const response = await api.patch(`/suppliers/${id}/toggle-status`);
    return response.data;
  },
};