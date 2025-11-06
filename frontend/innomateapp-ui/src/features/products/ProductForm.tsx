import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useProductStore } from "../../store/useProductStore.js";
import { useCategoryStore } from "@/store/useCategoryStore.js";
import { CreateProductDto, ProductDTO } from "../../types/product.js";
import Button from "@/components/ui/Button.js";
import Input from "@/components/ui/Input.js";
import { FormSelect } from "@/components/ui/Select.js";

// ✅ Zod schema for validation
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
  onSaved: () => void;
}

export default function ProductForm({ product, onSaved }: Props) {
  const { createProduct, updateProduct } = useProductStore();
  const { categories, fetchCategories } = useCategoryStore();
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    setValue,
    formState: { errors },
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

  const onSubmit = async (data: ProductFormData) => {
    try {
      setLoading(true);
      
      if (product?.productId) {
        // Update existing product
        await updateProduct(product.productId, data as ProductDTO);
      } else {
        // Create new product
        await createProduct(data as CreateProductDto);
      }
      
      // Reset form and notify parent
      reset();
      onSaved();
    } catch (err) {
      console.error("Error saving product:", err);
      alert("Failed to save product. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="space-y-4 bg-white dark:bg-gray-800 p-6 rounded-xl shadow-md border border-gray-200 dark:border-gray-700 max-w-lg mx-auto"
    >
      <h2 className="text-xl font-semibold text-gray-800 dark:text-gray-200 mb-4">
        {product ? "Update Product" : "Add Product"}
      </h2>

      {/* Product Name */}
      <Input
        label="Product Name"
        placeholder="Enter product name"
        {...register("name")}
        error={errors.name?.message}
      />

      {/* Category */}
      <FormSelect
        label="Category"
        placeholder="Select Category"
        options={[
          { value: 0, label: "Select Category" },
          ...categories.map((c) => ({
            value: c.categoryId,
            label: c.name,
          })),
        ]}
        value={watch("categoryId")}
        onValueChange={(value) => setValue("categoryId", value as number)}
        error={errors.categoryId?.message}
      />

      {/* SKU */}
      <Input
        label="SKU"
        placeholder="Enter SKU (optional)"
        {...register("sku")}
        error={errors.sku?.message}
      />

      {/* Default Sale Price */}
      <Input
        label="Default Sale Price"
        type="number"
        step="0.01"
        placeholder="Enter default sale price"
        {...register("defaultSalePrice", { valueAsNumber: true })}
        error={errors.defaultSalePrice?.message}
      />

      {/* Reorder Level */}
      <Input
        label="Reorder Level"
        type="number"
        placeholder="Enter reorder level"
        {...register("reorderLevel", { valueAsNumber: true })}
        error={errors.reorderLevel?.message}
      />

      {/* Is Active */}
      <div className="flex items-center gap-2 mt-2">
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

      {/* Submit Button */}
      <Button type="submit" loading={loading} className="w-full mt-4">
        {product ? "Update Product" : "Add Product"}
      </Button>
    </form>
  );
}