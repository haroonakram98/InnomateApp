import { useState,useEffect } from "react";
import ProductTable from "../features/products/ProductTable.js";
import ProductForm from "../features/products/ProductForm.js";
import { ProductDTO } from "../types/product.js";
import MainLayout from "@/components/layout/MainLayout.js";
import { useProductStore } from "@/store/useProductStore.js";
import Button from "@/components/ui/Button.js";

export default function ProductsPage() {
  const { products, fetchProducts, deleteProduct } = useProductStore();
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [modalOpen, setModalOpen] = useState(false);
  

  // Fetch products on mount
  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  const handleAdd = () => {
    setSelectedProduct(null);
    setModalOpen(true);
  };

  const handleEdit = (product: any) => {
    debugger
    setSelectedProduct(product);
    setModalOpen(true);
  };

  const handleDelete = async (productId: any) => {
    if (confirm("Are you sure you want to delete this product?")) {
      await deleteProduct(productId);
    }
  };

  const handleSaved = () => {
    setModalOpen(false);
    fetchProducts();
  };

  return (
    <MainLayout>
        <div className="p-4">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold text-gray-800 dark:text-gray-200">
          Products
        </h1>
        <Button onClick={handleAdd}>Add Product</Button>
      </div>

      {/* Products Table */}
      <div className="overflow-x-auto rounded-lg shadow-md">
        <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead className="bg-gray-100 dark:bg-gray-700">
            <tr>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">ID</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">Name</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">Category</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">SKU</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">Price</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">Reorder Level</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">Active</th>
              <th className="px-4 py-2 text-left text-sm font-medium text-gray-700 dark:text-gray-200">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            {products.map((p) => (
              <tr key={p.productId}>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.productId}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.name}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.categoryName || p.categoryId}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.sku}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.defaultSalePrice}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.reorderLevel}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200">{p.isActive ? "Yes" : "No"}</td>
                <td className="px-4 py-2 text-sm text-gray-700 dark:text-gray-200 flex gap-2">
                  <Button onClick={() => handleEdit(p)} size="sm">Edit</Button>
                  <Button onClick={() => handleDelete(p.productId)} size="sm" variant="danger">Delete</Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal */}
      {modalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 p-4">
          <div className="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-6 w-full max-w-lg relative">
            <button
              className="absolute top-2 right-2 text-gray-600 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white"
              onClick={() => setModalOpen(false)}
            >
              ✕
            </button>
            <ProductForm product={selectedProduct} onSaved={handleSaved} />
          </div>
        </div>
      )}
    </div>
    </MainLayout>
    
  );
}
