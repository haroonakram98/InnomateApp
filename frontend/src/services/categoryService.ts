// services/categoryService.ts
import { CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO } from "@/types/category.js";
import { categoryApi } from "@/api/categories.js";

class CategoryService {
  async getCategories(): Promise<CategoryDTO[]> {
    return await categoryApi.getAll();
  }

  async createCategory(payload: CreateCategoryDTO): Promise<CategoryDTO> {
    return await categoryApi.create(payload);
  }

  async updateCategory(id: number, payload: UpdateCategoryDTO): Promise<CategoryDTO> {
    // Convert UpdateCategoryDTO to Partial<CategoryDTO> for the API
    const updatePayload: Partial<CategoryDTO> = {
      name: payload.name,
      description: payload.description
    };
    return await categoryApi.update(id, updatePayload);
  }

  async deleteCategory(id: number): Promise<void> {
    await categoryApi.delete(id);
  }
}

export const categoryService = new CategoryService();