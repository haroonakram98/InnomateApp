// features/products/ProductForm.tsx
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { ProductDTO, CreateProductDto, UpdateProductDto } from "@/types/product.js";
import { useCategories } from '@/store/useCategoryStore.js';
import { useCategoryActions } from '@/store/useCategoryStore.js';

// Zod schema for validation
const productSchema = z.object({
  name: z.string().min(1, "Product name is required"),
  categoryId: z.number().min(1, "Category is required"),
  sku: z.string().optional(),
  defaultSalePrice: z.number().min(0, "Price must be >= 0"),
  reorderLevel: z.number().min(0, "Reorder level must be >= 0"),
  isActive: z.boolean().default(true),
});

type ProductFormData = z.infer<typeof productSchema>;

interface Props {
  product?: ProductDTO | null;
  onCreate?: (data: CreateProductDto) => void;
  onUpdate?: (data: UpdateProductDto) => void;
  onCancel: () => void;
}

export default function ProductForm({ product, onCreate, onUpdate, onCancel }: Props) {
  const categories = useCategories();
  const { fetchCategories } = useCategoryActions();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: "",
      categoryId: 0,
      sku: "",
      defaultSalePrice: 0,
      reorderLevel: 0,
      isActive: true,
    },
  });

  // Fetch categories on mount
  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  // Reset form when product changes
  useEffect(() => {
    if (product) {
      reset({
        name: product.name || "",
        categoryId: product.categoryId || 0,
        sku: product.sku || "",
        defaultSalePrice: product.defaultSalePrice || 0,
        reorderLevel: product.reorderLevel || 0,
        isActive: product.isActive ?? true,
      });
    } else {
      reset({
        name: "",
        categoryId: 0,
        sku: "",
        defaultSalePrice: 0,
        reorderLevel: 0,
        isActive: true,
      });
    }
  }, [product, reset]);

  const handleFormSubmit = async (data: ProductFormData) => {
    if (product && onUpdate) {
      // For update, include productId
      const updateData: UpdateProductDto = {
        ...data,
        productId: product.productId
      };
      await onUpdate(updateData);
    } else if (onCreate) {
      // For create, just use the form data
      await onCreate(data as CreateProductDto);
    }
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="p-6 space-y-4">
      {/* Product Name */}
      <div>
        <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
          Product Name *
        </label>
        <input
          type="text"
          {...register("name")}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
          placeholder="Enter product name"
        />
        {errors.name && (
          <p className="text-red-500 text-sm mt-1">{errors.name.message}</p>
        )}
      </div>

      {/* Category */}
      <div>
        <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
          Category *
        </label>
        <select
          {...register("categoryId", { valueAsNumber: true })}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
        >
          <option value={0}>Select Category</option>
          {categories.map((category) => (
            <option key={category.categoryId} value={category.categoryId}>
              {category.name}
            </option>
          ))}
        </select>
        {errors.categoryId && (
          <p className="text-red-500 text-sm mt-1">{errors.categoryId.message}</p>
        )}
      </div>

      {/* SKU */}
      <div>
        <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
          SKU
        </label>
        <input
          type="text"
          {...register("sku")}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
          placeholder="Enter SKU (optional)"
        />
        {errors.sku && (
          <p className="text-red-500 text-sm mt-1">{errors.sku.message}</p>
        )}
      </div>

      {/* Default Sale Price */}
      <div>
        <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
          Default Sale Price *
        </label>
        <input
          type="number"
          step="0.01"
          {...register("defaultSalePrice", { valueAsNumber: true })}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
          placeholder="Enter default sale price"
        />
        {errors.defaultSalePrice && (
          <p className="text-red-500 text-sm mt-1">{errors.defaultSalePrice.message}</p>
        )}
      </div>

      {/* Reorder Level */}
      <div>
        <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
          Reorder Level
        </label>
        <input
          type="number"
          {...register("reorderLevel", { valueAsNumber: true })}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100"
          placeholder="Enter reorder level"
        />
        {errors.reorderLevel && (
          <p className="text-red-500 text-sm mt-1">{errors.reorderLevel.message}</p>
        )}
      </div>

      {/* Stock Quantity (Read-only) */}
      <div>
        <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
          Stock Quantity
        </label>
        <input
          type="number"
          disabled
          value={((product?.stockSummary?.totalIn || 0) - (product?.stockSummary?.totalOut || 0))}
          className="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-500 dark:text-gray-400 cursor-not-allowed"
        />
        <p className="text-xs text-gray-500 mt-1">Stock quantity is managed through inventory</p>
      </div>

      {/* Is Active */}
      <div className="flex items-center gap-2">
        <input
          type="checkbox"
          id="isActive"
          {...register("isActive")}
          className="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 dark:focus:ring-blue-400 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600"
        />
        <label
          htmlFor="isActive"
          className="text-sm font-medium text-gray-700 dark:text-gray-300"
        >
          Active
        </label>
      </div>

      {/* Form Actions */}
      <div className="flex gap-3 pt-4">
        <button
          type="submit"
          disabled={isSubmitting}
          className="flex-1 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white py-2 px-4 rounded-lg transition-colors"
        >
          {isSubmitting ? 'Saving...' : (product ? 'Update Product' : 'Create Product')}
        </button>
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 bg-gray-600 hover:bg-gray-700 text-white py-2 px-4 rounded-lg transition-colors"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}