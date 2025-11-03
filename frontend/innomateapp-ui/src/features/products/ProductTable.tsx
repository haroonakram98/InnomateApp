import { useEffect } from "react";
import { productApi } from "../../api/products.js";
import { useProductStore } from "../../store/useProductStore.js";
import { ProductDTO } from "../../types/product.js";

export default function ProductTable({ onEdit }: { onEdit: (p: ProductDTO) => void }) {
  const { products, setProducts } = useProductStore();

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    const data = await productApi.getAll();
    setProducts(data);
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure to delete this product?")) {
      await productApi.delete(id);
      loadProducts();
    }
  };

  return (
    <div className="overflow-x-auto bg-white shadow rounded-xl">
      <table className="min-w-full text-sm">
        <thead className="bg-gray-100 text-gray-600">
          <tr>
            <th className="p-2 text-left">Name</th>
            <th>Category</th>
            <th>Sale Price</th>
            <th>Stock Value</th>
            <th className="text-center">Actions</th>
          </tr>
        </thead>
        <tbody>
          {products.map((p) => (
            <tr key={p.productId} className="border-b hover:bg-gray-50">
              <td className="p-2">{p.name}</td>
              <td>{p.categoryName}</td>
              <td>{p.defaultSalePrice}</td>
              <td>{p.totalValue}</td>
              <td className="text-center space-x-2">
                <button onClick={() => onEdit(p)} className="text-blue-500 hover:underline">Edit</button>
                <button onClick={() => handleDelete(p.productId)} className="text-red-500 hover:underline">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
