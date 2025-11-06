import axios from "@/lib/utils/axios.js";
import { SaleDTO } from "@/types/sale.js";
const BASE_URL = "/Sales";

export const getSales = async (): Promise<SaleDTO[]> => {
  const res = await axios.get(BASE_URL);
  return res.data;
};

export const getSaleById = async (id: number): Promise<SaleDTO> => {
  const r = await axios.get(`${BASE_URL}/${id}`);
  return r.data;
};

export const createSale = async (data: SaleDTO): Promise<SaleDTO> => {
  const res = await axios.post(BASE_URL, data);
  return res.data;
};

export const updateSale = async (id: number, data: SaleDTO): Promise<SaleDTO> => {
  const res = await axios.put(`${BASE_URL}/${id}`, data);
  return res.data;
};

export const deleteSale = async (id: number): Promise<void> => {
  await axios.delete(`${BASE_URL}/${id}`);
};
