// api/categories.ts
import axios from "@/lib/utils/axios.js";
import {
  CategoryDTO,
  CreateCategoryDTO,
  UpdateCategoryDTO
} from "@/types/category.js";

export const categoryApi = {
  getAll: async (search?: string): Promise<CategoryDTO[]> => {
    const { data } = await axios.get<CategoryDTO[]>("/categories", {
      params: { search }
    });
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

  update: async (id: number, payload: UpdateCategoryDTO): Promise<CategoryDTO> => {
    const { data } = await axios.put<CategoryDTO>(`/categories/${id}`, payload);
    return data;
  },

  delete: async (id: number): Promise<void> => {
    await axios.delete(`/categories/${id}`);
  },

  toggleStatus: async (id: number): Promise<{ isActive: boolean }> => {
    const { data } = await axios.patch(`/categories/${id}/toggle-status`);
    return data;
  },
};