import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useProductStore } from "../../store/useProductStore.js";
import { CreateProductDto, ProductDTO } from "../../types/product.js";
import Button from "@/components/ui/Button.js";
import Input from "@/components/ui/Input.js";
// ✅ Zod schema for validation
const productSchema = z.object({
  name: z.string().min(1, "Product name is required"),
  categoryId: z.number().min(1, "Category is required"),
  defaultSalePrice: z.number().min(0, "Price must be >= 0"),
  reorderLevel: z.number().min(0, "Reorder level must be >= 0"),
});

type ProductFormData = z.infer<typeof productSchema> & Partial<ProductDTO>;

interface Props {
  product?: ProductDTO | null;
  onSaved: () => void;
}

export default function ProductForm({ product, onSaved }: Props) {
  const { createProduct, updateProduct } = useProductStore();
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: product || {
      name: "",
      categoryId: 0,
      defaultSalePrice: 0,
      reorderLevel: 0,
      isActive: true,
      sku: "",
    },
  });

  useEffect(() => {
    // Reset form when editing a new product
    reset(product || {
      name: "",
      categoryId: 0,
      defaultSalePrice: 0,
      reorderLevel: 0,
      isActive: true,
      sku: "",
    });
  }, [product, reset]);

  const onSubmit = async (data: ProductFormData) => {
    try {
      setLoading(true);
      if (product?.productId) {
        const { productId, ...restData } = data;
        await updateProduct(product.productId, restData as ProductDTO);
      } else {
        await createProduct(data);
      }
      reset();
      onSaved();
    } catch (err) {
      console.error("Error saving product:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="space-y-4 bg-white dark:bg-gray-800 p-6 rounded-xl shadow-md border border-gray-200 max-w-lg mx-auto"
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

      {/* Category ID */}
      <Input
        label="Category ID"
        type="number"
        placeholder="Enter category ID"
        {...register("categoryId", { valueAsNumber: true })}
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
          className="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 dark:focus:ring-blue-400 dark:ring-offset-gray-800 focus:ring-2"
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
