import axios from "@/lib/utils/axios.js";
import { ProductDTO, CreateProductDto } from "../types/product.js";


export const productApi = {
  getAll: async (): Promise<ProductDTO[]> => {
    const res = await axios.get("/v1/Product");
    return res.data;
  },

  getById: async (id: number): Promise<ProductDTO> => {
    const res = await axios.get(`/v1/Product/${id}`);
    return res.data;
  },

  create: async (data: CreateProductDto): Promise<ProductDTO> => {
    const res = await axios.post("/v1/Product/", data);
    return res.data; // ✅ make sure backend returns the new product
  },

  update: async (id: number, data: ProductDTO): Promise<ProductDTO> => {
    const res = await axios.put(`/v1/Product/${id}`, data);
    return res.data; // ✅ backend should return updated product
  },

  delete: async (id: number): Promise<void> => {
    await axios.delete(`/v1/Product/${id}`);
  },
};
