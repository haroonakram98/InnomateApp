import axios from "@/lib/utils/axios.js"
import { PurchaseDTO, CreatePurchaseDTO } from '@/types/purchase.js';

export const purchaseService = {
  getPurchases: async (): Promise<PurchaseDTO[]> => {
    const response = await axios.get('/purchases');
    return response.data;
  },

  getPurchasesByDateRange: async (startDate: string, endDate: string, search?: string): Promise<PurchaseDTO[]> => {
    const response = await axios.get('/purchases', {
      params: { startDate, endDate, search }
    });
    return response.data;
  },

  getPurchaseById: async (id: number): Promise<PurchaseDTO> => {
    const response = await axios.get(`/purchases/${id}`);
    return response.data;
  },

  createPurchase: async (payload: CreatePurchaseDTO): Promise<PurchaseDTO> => {
    const response = await axios.post('/purchases', payload);
    return response.data;
  },

  receivePurchase: async (id: number): Promise<PurchaseDTO> => {
    const response = await axios.post(`/purchases/${id}/receive`);
    return response.data;
  },

  cancelPurchase: async (id: number): Promise<void> => {
    await axios.post(`/purchases/${id}/cancel`);
  },

  getNextPurchaseNumber: async (): Promise<string> => {
    const response = await axios.get('/purchases/next-purchase-no');
    return response.data.purchaseNo;
  },

  getNextBatchNumber: async (): Promise<string> => {
    const response = await axios.get('/purchases/next-batch-no');
    return response.data.batchNo;
  }
};