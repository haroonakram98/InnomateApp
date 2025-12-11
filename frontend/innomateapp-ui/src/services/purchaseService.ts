// src/services/purchaseService.ts
import axios from "@/lib/utils/axios.js"
import { PurchaseDTO, CreatePurchaseDTO, UpdatePurchaseDTO } from '@/types/purchase.js';

export const purchaseService = {
  // Get all purchases
  getPurchases: async (): Promise<PurchaseDTO[]> => {
    const response = await axios.get('/purchases');
    return response.data;
  },

  // Get purchases by date range
  getPurchasesByDateRange: async (startDate: string, endDate: string): Promise<PurchaseDTO[]> => {
    const response = await axios.get('/purchases', {
      params: { startDate, endDate }
    });
    return response.data;
  },

  // Get purchase by ID
  getPurchaseById: async (id: number): Promise<PurchaseDTO> => {
    const response = await axios.get(`/purchases/${id}`);
    return response.data;
  },

  // Create new purchase
  createPurchase: async (payload: CreatePurchaseDTO): Promise<PurchaseDTO> => {
    const response = await axios.post('/purchases', payload);
    return response.data;
  },

  // Receive purchase
  receivePurchase: async (id: number): Promise<PurchaseDTO> => {
    const response = await axios.post(`/purchases/${id}/receive`);
    return response.data;
  },

  // Cancel purchase
  cancelPurchase: async (id: number): Promise<void> => {
    const response = await axios.post(`/purchases/${id}/cancel`);
    return response.data;
  },

  // Get purchases by supplier
  getPurchasesBySupplier: async (supplierId: number): Promise<PurchaseDTO[]> => {
    const response = await axios.get(`/purchases/supplier/${supplierId}`);
    return response.data;
  },

  // Get next purchase number
  getNextPurchaseNumber: async (): Promise<string> => {
    const response = await axios.get('/purchases/next-purchase-no');
    return response.data.purchaseNo;
  },

  // Get next batch number
  getNextBatchNumber: async (): Promise<string> => {
    const response = await axios.get('/purchases/next-batch-no');
    return response.data.batchNo;
  },

  // Get pending purchases
  getPendingPurchases: async (): Promise<PurchaseDTO[]> => {
    const response = await axios.get('/purchases/pending');
    return response.data;
  },
};