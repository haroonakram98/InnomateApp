export interface ProductDTO {
  productId: number;
  name: string;
  categoryId: number;
  categoryName?: string;
  sku?: string;
  reorderLevel: number;
  isActive: boolean;
  defaultSalePrice: number;
  stockBalance?: number;
  averageCost?: number;
  totalValue?: number;
}

// ✅ Separate request DTOs
export type CreateProductDto = Omit<ProductDTO, "productId" | "isActive" | "categoryName" | "stockBalance" | "averageCost" | "totalValue">;
export type UpdateProductDto = Partial<CreateProductDto> & { productId: number };
