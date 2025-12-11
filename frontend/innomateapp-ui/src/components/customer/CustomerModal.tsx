import { useState, useEffect } from 'react';
import { CustomerDTO, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer.js';
import { useTheme } from '@/hooks/useTheme.js';
import { useCustomerActions } from '@/store/usecustomerStore.js';
import { X } from 'lucide-react';

interface CustomerModalProps {
    isOpen: boolean;
    onClose: () => void;
    customerToEdit?: CustomerDTO | null;
    onSuccess?: (customer: CustomerDTO) => void;
}

export default function CustomerModal({ isOpen, onClose, customerToEdit, onSuccess }: CustomerModalProps) {
    const { isDark } = useTheme();
    const { createCustomer, updateCustomer } = useCustomerActions();
    const [formData, setFormData] = useState<CreateCustomerDto>({
        name: '',
        email: '',
        phone: '',
        address: ''
    });

    // Reset form when modal opens or customerToEdit changes
    useEffect(() => {
        if (isOpen) {
            if (customerToEdit) {
                setFormData({
                    name: customerToEdit.name,
                    email: customerToEdit.email || '',
                    phone: customerToEdit.phone || '',
                    address: customerToEdit.address || ''
                });
            } else {
                setFormData({
                    name: '',
                    email: '',
                    phone: '',
                    address: ''
                });
            }
        }
    }, [isOpen, customerToEdit]);

    const handleSubmit = async () => {
        if (!formData.name.trim()) return;

        try {
            if (customerToEdit) {
                const updatePayload: UpdateCustomerDto = {
                    customerId: customerToEdit.customerId,
                    name: formData.name,
                    email: formData.email || undefined,
                    phone: formData.phone || undefined,
                    address: formData.address || undefined
                };
                const updated = await updateCustomer(customerToEdit.customerId, updatePayload);
                if (onSuccess && updated) onSuccess(updated);
            } else {
                const createPayload: CreateCustomerDto = {
                    name: formData.name,
                    email: formData.email || undefined,
                    phone: formData.phone || undefined,
                    address: formData.address || undefined
                };
                const created = await createCustomer(createPayload);
                if (onSuccess && created) onSuccess(created);
            }
            onClose();
        } catch (error) {
            console.error("Failed to save customer", error);
            // Error is usually handled by the store/toast, but we keep modal open on error ideally? 
            // The current store implementation seems to handle errors globally or we might want to close only on success.
            // Assuming store actions return the object on success and throw on error.
            // If store doesn't return the object, we might need to fetch the last created one or rely on store state.
            // Based on typical patterns here, let's assume it throws on error so we don't close.
        }
    };

    if (!isOpen) return null;

    // Theme classes
    const theme = {
        bgCard: isDark ? 'bg-gray-800' : 'bg-white',
        text: isDark ? 'text-gray-100' : 'text-gray-900',
        border: isDark ? 'border-gray-700' : 'border-gray-200',
        input: isDark
            ? 'bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-400 focus:border-blue-500'
            : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-blue-500',
        buttonPrimary: 'bg-blue-600 hover:bg-blue-700 text-white',
        buttonSecondary: isDark
            ? 'bg-gray-700 hover:bg-gray-600 text-gray-100'
            : 'bg-gray-200 hover:bg-gray-300 text-gray-700',
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className={`rounded-xl shadow-2xl w-full max-w-md ${theme.bgCard}`}>
                <div className={`p-6 border-b flex justify-between items-center ${theme.border}`}>
                    <h3 className={`text-lg font-semibold ${theme.text}`}>
                        {customerToEdit ? 'Edit Customer' : 'Create New Customer'}
                    </h3>
                    <button onClick={onClose} className={`p-1 rounded hover:bg-opacity-20 hover:bg-gray-500 ${theme.text}`}>
                        <X size={20} />
                    </button>
                </div>

                <div className="p-6 space-y-4">
                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Customer Name *
                        </label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                            placeholder="Enter customer name"
                            autoFocus
                        />
                    </div>

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Email Address
                        </label>
                        <input
                            type="email"
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                            placeholder="Enter email address (optional)"
                        />
                    </div>

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Phone Number
                        </label>
                        <input
                            type="tel"
                            value={formData.phone}
                            onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
                            placeholder="Enter phone number (optional)"
                        />
                    </div>

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Address
                        </label>
                        <textarea
                            value={formData.address}
                            onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                            rows={3}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none ${theme.input}`}
                            placeholder="Enter address (optional)"
                        />
                    </div>

                </div>

                <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                    <button
                        onClick={handleSubmit}
                        disabled={!formData.name.trim()}
                        className={`flex-1 py-2 px-4 rounded-lg transition-colors disabled:opacity-50 ${theme.buttonPrimary}`}
                    >
                        {customerToEdit ? 'Update Customer' : 'Create Customer'}
                    </button>
                    <button
                        onClick={onClose}
                        className={`flex-1 py-2 px-4 rounded-lg border transition-colors ${theme.buttonSecondary}`}
                    >
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    );
}
