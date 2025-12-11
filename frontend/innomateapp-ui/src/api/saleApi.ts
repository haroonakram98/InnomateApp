// src/api/saleApi.ts
import axios from "@/lib/utils/axios.js";
import type { SaleDTO, CreateSaleDTO } from "@/types/sale.js";

const BASE_URL = "/Sales";

export const getSales = async (): Promise<SaleDTO[]> => {
  const res = await axios.get(BASE_URL);
  return res.data;
};

export const getSaleById = async (id: number): Promise<SaleDTO> => {
  const res = await axios.get(`${BASE_URL}/${id}`);
  return res.data;
};

export const createSale = async (data: CreateSaleDTO): Promise<SaleDTO> => {
  debugger
  const res = await axios.post(BASE_URL, data);
  return res.data;
};

export const updateSale = async (id: number, data: Partial<CreateSaleDTO>): Promise<SaleDTO> => {
  const res = await axios.put(`${BASE_URL}/${id}`, data);
  return res.data;
};

export const deleteSale = async (id: number): Promise<void> => {
  await axios.delete(`${BASE_URL}/${id}`);
};

// Additional API functions for enhanced functionality
export const getSalesByDateRange = async (startDate: string, endDate: string): Promise<SaleDTO[]> => {
  const res = await axios.get(`${BASE_URL}/date-range`, {
    params: { startDate, endDate }
  });
  return res.data;
};

export const getSalesByCustomer = async (customerId: number): Promise<SaleDTO[]> => {
  const res = await axios.get(`${BASE_URL}/customer/${customerId}`);
  return res.data;
};

export const getSalesSummary = async (startDate?: string, endDate?: string): Promise<any> => {
  const res = await axios.get(`${BASE_URL}/summary`, {
    params: { startDate, endDate }
  });
  return res.data;
};

export const getNextInvoiceNumber = async (): Promise<string> => {
  const res = await axios.get(`${BASE_URL}/next-invoice-no`);
  return res.data.invoiceNo;
};

// Payment related APIs
export const addPaymentToSale = async (saleId: number, paymentData: {
  paymentMethod: string;
  amount: number;
  referenceNo?: string;
}): Promise<SaleDTO> => {
  const res = await axios.post(`${BASE_URL}/${saleId}/payments`, paymentData);
  return res.data;
};

export const getSalePayments = async (saleId: number): Promise<any[]> => {
  const res = await axios.get(`${BASE_URL}/${saleId}/payments`);
  return res.data;
};

// Analytics and Reports
export const getSalesReport = async (params: {
  startDate?: string;
  endDate?: string;
  customerId?: number;
  productId?: number;
}): Promise<any> => {
  const res = await axios.get(`${BASE_URL}/report`, { params });
  return res.data;
};

export const getTopSellingProducts = async (limit: number = 10): Promise<any[]> => {
  const res = await axios.get(`${BASE_URL}/analytics/top-products`, {
    params: { limit }
  });
  return res.data;
};

export const getSalesTrend = async (period: 'daily' | 'weekly' | 'monthly' = 'monthly'): Promise<any[]> => {
  const res = await axios.get(`${BASE_URL}/analytics/trend`, {
    params: { period }
  });
  return res.data;
};