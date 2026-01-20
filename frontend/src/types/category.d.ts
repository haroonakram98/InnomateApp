// types/category.ts
export interface CategoryDTO {
  categoryId: number;
  name: string;
  description?: string;
}

export interface CreateCategoryDTO {
  name: string;
  description?: string;
}

export interface UpdateCategoryDTO {
  name: string;
  description?: string;
}