// services/productService.ts
import { ProductDTO, CreateProductDto, UpdateProductDto, ProductStockDto } from "@/types/product.js";
import { productApi } from "@/api/products.js";

class ProductService {
  async getProducts(): Promise<ProductDTO[]> {
    return await productApi.getAll();
  }

  async fetchProductsForSale(): Promise<ProductStockDto[]> {
    return await productApi.getAllForSales();
  }

  async getLookup(): Promise<import("@/types/product.js").ProductLookupDTO[]> {
    return await productApi.getLookup();
  }

  async createProduct(payload: CreateProductDto): Promise<ProductDTO> {
    return await productApi.create(payload);
  }

  async updateProduct(payload: UpdateProductDto): Promise<ProductDTO> {
    // The payload already contains productId, pass it directly
    return await productApi.update(payload.productId, payload);
  }

  async deleteProduct(id: number): Promise<void> {
    await productApi.delete(id);
  }
}

export const productService = new ProductService();