// types/category.d.ts
export interface CategoryDTO {
  categoryId: number;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateCategoryDTO {
  name: string;
  description?: string;
}

export interface UpdateCategoryDTO extends CreateCategoryDTO {
  categoryId: number;
}