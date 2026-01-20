// api/products/index.ts
import axios from "@/lib/utils/axios.js";
import { ProductDTO, CreateProductDto, UpdateProductDto, ProductStockDto, ProductLookupDTO } from "../types/product.js";

export const productApi = {
  getAll: async (): Promise<ProductDTO[]> => {
    const res = await axios.get("/Product");
    return res.data;
  },
  getAllForSales: async (): Promise<ProductStockDto[]> => {
    const res = await axios.get<any[]>("/Product/for-sale");
    // Map backend response (currentStock) to frontend model (availableStock)
    return res.data.map(p => ({
      ...p,
      stockSummary: undefined, // Backend doesn't send this nested object anymore
      availableStock: p.currentStock || 0,
      availableQuantity: p.currentStock || 0, // Mapping both for compatibility
      stockStatus: p.stockStatus
    })) as ProductStockDto[];
  },

  getLookup: async (): Promise<ProductLookupDTO[]> => {
    const res = await axios.get<ProductLookupDTO[]>("/Product/lookup");
    return res.data;
  },

  getById: async (id: number): Promise<ProductDTO> => {
    const res = await axios.get(`/Product/${id}`);
    return res.data;
  },

  create: async (data: CreateProductDto): Promise<ProductDTO> => {
    const res = await axios.post("/Product/", data);
    return res.data;
  },

  update: async (id: number, data: UpdateProductDto): Promise<ProductDTO> => {
    const res = await axios.put(`/Product/${id}`, data);
    return res.data;
  },

  delete: async (id: number): Promise<void> => {
    await axios.delete(`/Product/${id}`);
  },
};