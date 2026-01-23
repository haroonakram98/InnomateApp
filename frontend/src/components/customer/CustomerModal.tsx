import { useState, useEffect } from 'react';
import { CustomerDTO, CreateCustomerDto, UpdateCustomerDto } from '@/types/customer.js';
import { useTheme } from '@/hooks/useTheme.js';
import {
    useCustomerActions,
    useValidationErrors,
    useCustomersError
} from '@/store/useCustomerStore.js';
import { X } from 'lucide-react';
import { customerSchema } from '@/lib/schemas/customerSchema.js';
import { z } from 'zod';

interface CustomerModalProps {
    isOpen: boolean;
    onClose: () => void;
    customerToEdit?: CustomerDTO | null;
    onSuccess?: (customer: CustomerDTO) => void;
}

export default function CustomerModal({ isOpen, onClose, customerToEdit, onSuccess }: CustomerModalProps) {
    const { isDark } = useTheme();
    const error = useCustomersError();
    const validationErrors = useValidationErrors();
    const {
        createCustomer,
        updateCustomer,
        setValidationErrors,
        clearError
    } = useCustomerActions();

    const [formData, setFormData] = useState<CreateCustomerDto>({
        name: '',
        email: '',
        phone: '',
        address: ''
    });

    // Reset form when modal opens or customerToEdit changes
    useEffect(() => {
        if (isOpen) {
            clearError();
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
    }, [isOpen, customerToEdit, clearError]);

    // Live validation: Clear errors as user fixes them
    useEffect(() => {
        if (validationErrors) {
            const result = customerSchema.safeParse(formData);
            if (result.success) {
                setValidationErrors(null);
            } else {
                const formattedErrors: Record<string, string[]> = {};
                result.error.errors.forEach((err) => {
                    const path = err.path.join('.');
                    if (!formattedErrors[path]) formattedErrors[path] = [];
                    formattedErrors[path].push(err.message);
                });
                setValidationErrors(formattedErrors);
            }
        }
    }, [formData, setValidationErrors]);

    const getFieldError = (fieldName: string) => {
        if (!validationErrors) return null;
        const lower = fieldName.toLowerCase();
        const capitalized = fieldName.charAt(0).toUpperCase() + fieldName.slice(1);

        const err =
            validationErrors[lower] ||
            validationErrors[capitalized] ||
            validationErrors[`CustomerDto.${capitalized}`] ||
            validationErrors[`CustomerDto.${lower}`];

        return err ? err[0] : null;
    };

    const handleSubmit = async () => {
        try {
            // 🕵️‍♂️ Client Side Validation with Zod
            customerSchema.parse(formData);

            if (customerToEdit) {
                const updatePayload: UpdateCustomerDto = {
                    customerId: customerToEdit.customerId,
                    name: formData.name,
                    email: formData.email || undefined,
                    phone: formData.phone || undefined,
                    address: formData.address || undefined
                };
                await updateCustomer(customerToEdit.customerId, updatePayload);
            } else {
                await createCustomer(formData);
            }
            onClose();
        } catch (err) {
            if (err instanceof z.ZodError) {
                const formattedErrors: Record<string, string[]> = {};
                err.errors.forEach((error) => {
                    const path = error.path.join('.');
                    if (!formattedErrors[path]) formattedErrors[path] = [];
                    formattedErrors[path].push(error.message);
                });
                setValidationErrors(formattedErrors);
            }
            // Server errors are handled by the store
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
                    {/* Modal Error Summary */}
                    {error && !validationErrors && (
                        <div className={`p-3 rounded-lg text-xs border ${isDark ? 'bg-red-900/20 border-red-800 text-red-200' : 'bg-red-50 border-red-200 text-red-700'
                            }`}>
                            {error}
                        </div>
                    )}

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Customer Name *
                        </label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${getFieldError('Name') ? 'border-red-500 focus:ring-red-500' : theme.input
                                }`}
                            placeholder="Enter customer name"
                            autoFocus
                        />
                        {getFieldError('Name') && (
                            <p className="mt-1 text-xs text-red-500">{getFieldError('Name')}</p>
                        )}
                    </div>

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Email Address
                        </label>
                        <input
                            type="email"
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${getFieldError('Email') ? 'border-red-500 focus:ring-red-500' : theme.input
                                }`}
                            placeholder="Enter email address (optional)"
                        />
                        {getFieldError('Email') && (
                            <p className="mt-1 text-xs text-red-500">{getFieldError('Email')}</p>
                        )}
                    </div>

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Phone Number
                        </label>
                        <input
                            type="tel"
                            value={formData.phone}
                            onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${getFieldError('Phone') ? 'border-red-500 focus:ring-red-500' : theme.input
                                }`}
                            placeholder="Enter phone number (optional)"
                        />
                        {getFieldError('Phone') && (
                            <p className="mt-1 text-xs text-red-500">{getFieldError('Phone')}</p>
                        )}
                    </div>

                    <div>
                        <label className={`block text-sm font-medium mb-1 ${theme.text}`}>
                            Address
                        </label>
                        <textarea
                            value={formData.address}
                            onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                            rows={3}
                            className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none ${getFieldError('Address') ? 'border-red-500 focus:ring-red-500' : theme.input
                                }`}
                            placeholder="Enter address (optional)"
                        />
                        {getFieldError('Address') && (
                            <p className="mt-1 text-xs text-red-500">{getFieldError('Address')}</p>
                        )}
                    </div>
                </div>

                <div className={`flex space-x-3 p-6 border-t ${theme.border}`}>
                    <button
                        onClick={handleSubmit}
                        className={`flex-1 py-2 px-4 rounded-lg transition-colors ${theme.buttonPrimary}`}
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
