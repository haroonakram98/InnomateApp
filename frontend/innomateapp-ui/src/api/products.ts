import axios from "@/lib/utils/axios.js";
import { ProductDTO, CreateProductDto } from "../types/product.js";


export const productApi = {
  getAll: async (): Promise<ProductDTO[]> => {
    const res = await axios.get("/Product");
    return res.data;
  },

  getById: async (id: number): Promise<ProductDTO> => {
    const res = await axios.get(`/Product/${id}`);
    return res.data;
  },

  create: async (data: CreateProductDto): Promise<ProductDTO> => {
    const res = await axios.post("/Product/", data);
    return res.data; // ✅ make sure backend returns the new product
  },

  update: async (id: number, data: ProductDTO): Promise<ProductDTO> => {
    const res = await axios.put(`/Product/${id}`, data);
    return res.data; // ✅ backend should return updated product
  },

  delete: async (id: number): Promise<void> => {
    await axios.delete(`/Product/${id}`);
  },
};
