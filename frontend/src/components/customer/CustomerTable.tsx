import React from "react";
import { CustomerDTO } from "@/types/customer.js";
import Button from "@/components/ui/Button.js";

type Props = {
  customers: CustomerDTO[];
  loading?: boolean;
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
};

export const CustomerTable: React.FC<Props> = ({ customers, loading, onEdit, onDelete }) => {
  if (loading) {
    return <div className="p-4">Loading...</div>;
  }

  if (!customers.length) {
    return <div className="p-4 text-sm text-gray-500 dark:text-gray-400">No customers found.</div>;
  }

  return (
    <div className="overflow-x-auto bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <table className="w-full min-w-[700px]">
        <thead className="text-left border-b dark:border-gray-700">
          <tr>
            <th className="p-3">Name</th>
            <th className="p-3">Email</th>
            <th className="p-3">Phone</th>
            <th className="p-3">Address</th>
            <th className="p-3 text-center">Actions</th>
          </tr>
        </thead>
        <tbody>
          {customers.map((c) => (
            <tr key={c.customerId} className="border-b last:border-b-0 dark:border-gray-700">
              <td className="p-3">{c.name}</td>
              <td className="p-3">{c.email}</td>
              <td className="p-3">{c.phone ?? "-"}</td>
              <td className="p-3">{c.address ?? "-"}</td>
              <td className="p-3 text-center">
                <div className="flex items-center justify-center gap-2">
                  <Button variant="outline" onClick={() => onEdit(c.customerId)}>Edit</Button>
                  <Button variant="danger" onClick={() => onDelete(c.customerId)}>Delete</Button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
