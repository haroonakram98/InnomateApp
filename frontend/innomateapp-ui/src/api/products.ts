// api/products/index.ts
import axios from "@/lib/utils/axios.js";
import { ProductDTO, CreateProductDto, UpdateProductDto,ProductStockDto } from "../types/product.js";

export const productApi = {
  getAll: async (): Promise<ProductDTO[]> => {
    const res = await axios.get("/Product");
    return res.data;
  },
  getAllForSales: async (): Promise<ProductStockDto[]> => {
    const res = await axios.get("/Product/for-sale");
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
    debugger;
    await axios.delete(`/Product/${id}`);
  },
};