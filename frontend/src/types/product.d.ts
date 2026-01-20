import { StockSummaryDto } from "./stock.js";

// types/product.ts
export interface ProductDTO {
  productId: number;
  name: string;
  categoryId: number;
  categoryName?: string;
  sku?: string;
  reorderLevel: number;
  isActive: boolean;
  defaultSalePrice: number;
  currentStock: number;
  stockStatus: string;
  totalValue: number;
}

export interface ProductLookupDTO {
  productId: number;
  name: string;
  sku?: string;
  defaultSalePrice: number;
}

// Create product doesn't need productId
export type CreateProductDto = Omit<ProductDTO, "productId">;

// Update product should be partial (all fields optional) but requires productId
export type UpdateProductDto = Partial<Omit<ProductDTO, "productId">> & {
  productId: number
};

export interface ProductStockDto extends ProductDTO {
  availableStock: number;
  availableQuantity: number;
  stockStatus: string; // e.g. "Low Stock" | "In Stock"
}