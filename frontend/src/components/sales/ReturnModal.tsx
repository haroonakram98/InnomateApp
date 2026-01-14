import React, { useState } from 'react';
import { useForm, useFieldArray } from 'react-hook-form';
import { X, Save, AlertTriangle } from 'lucide-react';
import { SaleDTO } from '@/types/sale.js';
import { returnService, CreateReturnRequest } from '@/services/returnService.js';
import { useTheme } from '@/hooks/useTheme.js';

interface ReturnModalProps {
    sale: SaleDTO;
    onClose: (refresh?: boolean) => void;
}

export default function ReturnModal({ sale, onClose }: ReturnModalProps) {
    const { isDark } = useTheme();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string>('');

    const { register, control, handleSubmit, watch, setValue } = useForm<CreateReturnRequest>({
        defaultValues: {
            saleId: sale.saleId,
            reason: '',
            returnDetails: []
        }
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: 'returnDetails'
    });

    // Theme classes
    const theme = {
        bg: isDark ? 'bg-gray-800' : 'bg-white',
        text: isDark ? 'text-gray-100' : 'text-gray-900',
        textSecondary: isDark ? 'text-gray-400' : 'text-gray-600',
        input: isDark ? 'bg-gray-700 border-gray-600 text-gray-100' : 'bg-white border-gray-300 text-gray-900',
        border: isDark ? 'border-gray-700' : 'border-gray-200',
    };

    // Helper to toggle product selection
    const toggleProduct = (productId: number, maxQty: number) => {
        const index = fields.findIndex(f => f.productId === productId);
        if (index >= 0) {
            remove(index);
        } else {
            append({ productId, quantity: 1 });
        }
    };

    // Calculate balance due for return logic
    const totalPaid = sale.payments?.reduce((sum, p) => sum + (p.amount || 0), 0) || 0;
    const balanceDue = sale.totalAmount - totalPaid;

    const watchedDetails = watch('returnDetails');

    // Calculate return breakdown
    const calculateReturnSummary = () => {
        const totalReturnValue = watchedDetails.reduce((sum, item) => {
            const saleDetail = sale.saleDetails?.find(sd => sd.productId === item.productId);
            if (!saleDetail) return sum;

            // Calculate effective unit price (after discount)
            const lineTotal = (saleDetail.unitPrice * saleDetail.quantity) - (saleDetail.discount || 0);
            const effectiveUnitPrice = lineTotal / saleDetail.quantity;

            return sum + (item.quantity * effectiveUnitPrice);
        }, 0);

        const balanceAdjustment = Math.min(totalReturnValue, balanceDue);
        const netRefund = Math.max(0, totalReturnValue - balanceAdjustment);

        return { totalReturnValue, balanceAdjustment, netRefund };
    };

    const summary = calculateReturnSummary();

    const onSubmit = async (data: CreateReturnRequest) => {
        if (data.returnDetails.length === 0) {
            setError('Please select at least one item to return.');
            return;
        }

        setLoading(true);
        setError('');

        try {
            await returnService.createReturn(data);
            onClose(true);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to process return');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-[60] p-4">
            <div className={`rounded-lg shadow-xl w-full max-w-3xl flex flex-col max-h-[90vh] ${theme.bg}`}>
                {/* Header */}
                <div className={`p-6 border-b ${theme.border}`}>
                    <div className="flex justify-between items-start">
                        <div>
                            <h3 className={`text-xl font-bold ${theme.text}`}>Process Return</h3>
                            <div className="flex items-center gap-2 mt-1">
                                <span className={`text-sm ${theme.textSecondary}`}>Sale #{sale.saleId}</span>
                                <span className={theme.textSecondary}>•</span>
                                <span className={`text-sm font-medium ${theme.text}`}>
                                    {sale.customer?.name || 'Walk-In Customer'}
                                </span>
                            </div>
                        </div>
                        <button onClick={() => onClose()} className={`p-2 rounded hover:bg-gray-100 dark:hover:bg-gray-700 ${theme.text}`}>
                            <X size={24} />
                        </button>
                    </div>
                </div>

                {/* Body */}
                <div className="flex-1 overflow-auto p-6">
                    {error && (
                        <div className="mb-4 p-3 bg-red-100 text-red-800 rounded flex items-center gap-2">
                            <AlertTriangle size={18} />
                            {error}
                        </div>
                    )}

                    <div className="mb-4">
                        <label className={`block text-sm font-medium mb-1 ${theme.textSecondary}`}>Return Reason</label>
                        <textarea
                            {...register('reason')}
                            className={`w-full p-2 rounded border focus:ring-2 focus:ring-blue-500 ${theme.input} h-20 resize-none`}
                            placeholder="Why is the customer returning these items?"
                        />
                    </div>

                    <table className="w-full">
                        <thead className={`text-left text-sm font-semibold border-b ${theme.border} ${theme.textSecondary}`}>
                            <tr>
                                <th className="pb-3 pl-2">Select</th>
                                <th className="pb-3">Product</th>
                                <th className="pb-3 text-right">Sold Qty</th>
                                <th className="pb-3 text-right">Unit Price</th>
                                <th className="pb-3 text-right w-24">Return Qty</th>
                                <th className="pb-3 text-right">Refund</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
                            {sale.saleDetails?.map((detail) => {
                                const isSelected = watchedDetails.some(d => d.productId === detail.productId);
                                const returnItem = watchedDetails.find(d => d.productId === detail.productId);

                                return (
                                    <tr key={detail.productId} className="group">
                                        <td className="py-3 pl-2">
                                            <input
                                                type="checkbox"
                                                checked={isSelected}
                                                onChange={() => toggleProduct(detail.productId, detail.quantity)}
                                                className="w-5 h-5 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                                            />
                                        </td>
                                        <td className={`py-3 ${theme.text}`}>{detail.productName}</td>
                                        <td className={`py-3 text-right ${theme.text}`}>{detail.quantity}</td>
                                        <td className={`py-3 text-right ${theme.text}`}>${detail.unitPrice.toFixed(2)}</td>
                                        <td className="py-3 text-right">
                                            {isSelected && (
                                                <input
                                                    type="number"
                                                    min="0"
                                                    max={detail.quantity}
                                                    value={returnItem?.quantity?.toString() ?? '0'}
                                                    onFocus={(e) => e.target.select()}
                                                    onChange={(e) => {
                                                        const rawValue = e.target.value;
                                                        // Handle empty input as 0
                                                        let val = rawValue === '' ? 0 : parseFloat(rawValue);

                                                        // Ensure valid range
                                                        if (isNaN(val)) val = 0;
                                                        if (val < 0) val = 0;
                                                        if (val > detail.quantity) val = detail.quantity;

                                                        const idx = fields.findIndex(f => f.productId === detail.productId);
                                                        if (idx >= 0) {
                                                            setValue(`returnDetails.${idx}.quantity`, val);
                                                        }
                                                    }}
                                                    className={`w-20 p-1 text-right rounded border focus:ring-2 focus:ring-blue-500 outline-none ${theme.input} [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-auto [&::-webkit-inner-spin-button]:appearance-auto`}
                                                />
                                            )}
                                        </td>
                                        <td className={`py-3 text-right font-medium ${theme.text}`}>
                                            {isSelected ? `$${(((returnItem?.quantity || 0) * (detail.unitPrice * detail.quantity - (detail.discount || 0)) / detail.quantity)).toFixed(2)}` : '-'}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>

                {/* Footer */}
                <div className={`p-6 border-t ${theme.border}`}>
                    <div className="flex flex-col gap-2 mb-4">
                        <div className="flex justify-between text-sm">
                            <span className={theme.textSecondary}>Return Value:</span>
                            <span className={theme.text}>${summary.totalReturnValue.toFixed(2)}</span>
                        </div>
                        {summary.balanceAdjustment > 0 && (
                            <div className="flex justify-between text-sm text-orange-600">
                                <span>Balance Adjustment:</span>
                                <span>-${summary.balanceAdjustment.toFixed(2)}</span>
                            </div>
                        )}
                        <div className="flex justify-between text-lg font-bold border-t pt-2 mt-1">
                            <span className={theme.text}>Net Refund:</span>
                            <span className="text-blue-600">${summary.netRefund.toFixed(2)}</span>
                        </div>
                    </div>

                    <div className="flex justify-end gap-3">
                        <button
                            onClick={() => onClose()}
                            className="px-4 py-2 rounded border text-gray-700 hover:bg-gray-50 border-gray-300 dark:border-gray-600 dark:text-gray-300 dark:hover:bg-gray-700"
                            disabled={loading}
                        >
                            Cancel
                        </button>
                        <button
                            onClick={handleSubmit(onSubmit)}
                            disabled={loading || watchedDetails.length === 0}
                            className="px-4 py-2 rounded bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50 flex items-center gap-2"
                        >
                            <Save size={18} />
                            {loading ? 'Processing...' : 'Confirm Return'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
