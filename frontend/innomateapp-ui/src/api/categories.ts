// api/categories/index.ts
import axios from "@/lib/utils/axios.js";
import { CategoryDTO, CreateCategoryDTO } from "@/types/category.js";

export const categoryApi = {
  getAll: async (): Promise<CategoryDTO[]> => {
    const { data } = await axios.get<CategoryDTO[]>("/categories");
    return data;
  },

  getById: async (id: number): Promise<CategoryDTO> => {
    const { data } = await axios.get<CategoryDTO>(`/categories/${id}`);
    return data;
  },

  create: async (payload: CreateCategoryDTO): Promise<CategoryDTO> => {
    const { data } = await axios.post<CategoryDTO>("/categories", payload);
    return data;
  },

  update: async (id: number, payload: Partial<CategoryDTO>): Promise<CategoryDTO> => {
    const { data } = await axios.put<CategoryDTO>(`/categories/${id}`, payload);
    return data;
  },

  delete: async (id: number): Promise<void> => {
    await axios.delete(`/categories/${id}`);
  },
};