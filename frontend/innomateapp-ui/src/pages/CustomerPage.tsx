import React, { useEffect, useState } from "react";
import MainLayout from "@/components/layout/MainLayout.js";
import Button from "@/components/ui/Button.js";
import {CustomerTable} from "@/components/customer/CustomerTable.js";
import {CustomerForm} from "@/components/customer/CustomerForm.js";
import { useCustomerStore } from "@/store/usecustomerStore.js";

const CustomerPage: React.FC = () => {
  const { customers, fetchCustomers, loading, deleteCustomer } = useCustomerStore();
  const [modalOpen, setModalOpen] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);

  useEffect(() => {
    fetchCustomers();
  }, [fetchCustomers]);

  const openAdd = () => {
    setEditId(null);
    setModalOpen(true);
  };

  const openEdit = (id: number) => {
    setEditId(id);
    setModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    const ok = confirm("Are you sure you want to delete this customer?");
    if (!ok) return;
    await deleteCustomer(id);
  };

  return (
    <MainLayout>
      <div className="p-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">Customers</h2>
          <Button onClick={openAdd}>+ Add Customer</Button>
        </div>

        <CustomerTable
          customers={customers}
          loading={loading}
          onEdit={openEdit}
          onDelete={handleDelete}
        />

        {modalOpen && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
            <div className="w-full max-w-2xl p-6 bg-white dark:bg-gray-800 rounded-lg shadow-lg">
              <div className="flex items-center justify-between mb-4">
                <h3 className="text-lg font-medium">{editId ? "Edit Customer" : "Add Customer"}</h3>
                <button onClick={() => setModalOpen(false)} className="text-gray-500">✕</button>
              </div>

              <CustomerForm
                customerId={editId}
                onClose={() => {
                  setModalOpen(false);
                  fetchCustomers();
                }}
              />
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
};

export default CustomerPage;
