
import React from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Calendar, User, FileText, CreditCard, DollarSign, Package } from 'lucide-react';
import { SaleDTO } from '@/types/sale.js';
import { useTheme } from '@/hooks/useTheme.js';

interface SaleDetailsModalProps {
    sale: SaleDTO;
    onClose: () => void;
}

export default function SaleDetailsModal({ sale, onClose }: SaleDetailsModalProps) {
    const { isDark } = useTheme();

    const theme = {
        bg: isDark ? 'bg-gray-900' : 'bg-white',
        text: isDark ? 'text-gray-100' : 'text-gray-900',
        textSecondary: isDark ? 'text-gray-400' : 'text-gray-600',
        border: isDark ? 'border-gray-700' : 'border-gray-200',
        headerBg: isDark ? 'bg-gray-800' : 'bg-gray-50',
        rowHover: isDark ? 'hover:bg-gray-800' : 'hover:bg-gray-50',
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'PKR', // Assuming PKR based on context "Rs"
        }).format(amount);
    };

    // Calculate totals for display consistency
    const totalPaid = sale.payments?.reduce((sum, p) => sum + (p.amount || 0), 0) || 0;
    const balanceDue = sale.totalAmount - totalPaid;
    const isPaid = balanceDue <= 0;

    return (
        <AnimatePresence>
            <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                <motion.div
                    initial={{ opacity: 0, scale: 0.95 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.95 }}
                    className={`${theme.bg} w-full max-w-4xl rounded-xl shadow-2xl overflow-hidden max-h-[90vh] flex flex-col`}
                >
                    {/* Header */}
                    <div className={`${theme.headerBg} p-6 border-b ${theme.border} flex justify-between items-start`}>
                        <div>
                            <div className="flex items-center gap-3 mb-2">
                                <h2 className={`text-2xl font-bold ${theme.text}`}>Sale Details</h2>
                                <span className={`px-3 py-1 rounded-full text-xs font-medium ${isPaid
                                    ? 'bg-green-100 text-green-800'
                                    : totalPaid > 0
                                        ? 'bg-yellow-100 text-yellow-800'
                                        : 'bg-gray-100 text-gray-800'
                                    }`}>
                                    {isPaid ? 'Paid' : totalPaid > 0 ? 'Partial' : 'Pending'}
                                </span>
                            </div>
                            <div className={`flex items-center gap-4 text-sm ${theme.textSecondary}`}>
                                <span className="flex items-center gap-1">
                                    <FileText size={14} />
                                    {sale.invoiceNo || `Sale #${sale.saleId}`}
                                </span>
                                <span className="flex items-center gap-1">
                                    <Calendar size={14} />
                                    {formatDate(sale.saleDate)}
                                </span>
                            </div>
                        </div>
                        <button
                            onClick={onClose}
                            className={`p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors ${theme.textSecondary}`}
                        >
                            <X size={24} />
                        </button>
                    </div>

                    <div className="overflow-y-auto p-6 space-y-8">
                        {/* Customer Info */}
                        <div className={`p-4 rounded-lg border ${theme.border}`}>
                            <h3 className={`text-sm font-semibold mb-3 ${theme.text} flex items-center gap-2`}>
                                <User size={16} /> Customer Information
                            </h3>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                    <p className={`text-xs ${theme.textSecondary}`}>Customer Name</p>
                                    <p className={`font-medium ${theme.text}`}>{sale.customerName || sale.customer?.name || 'Walk In'}</p>
                                </div>
                                {sale.customer?.phone && (
                                    <div>
                                        <p className={`text-xs ${theme.textSecondary}`}>Phone</p>
                                        <p className={`font-medium ${theme.text}`}>{sale.customer.phone}</p>
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* Line Items */}
                        <div>
                            <h3 className={`text-sm font-semibold mb-3 ${theme.text} flex items-center gap-2`}>
                                <Package size={16} /> Order Items
                            </h3>
                            <div className={`border rounded-lg overflow-hidden ${theme.border}`}>
                                <table className="w-full text-sm">
                                    <thead className={`bg-gray-50 dark:bg-gray-800 border-b ${theme.border}`}>
                                        <tr className={theme.textSecondary}>
                                            <th className="px-4 py-3 text-left">Product</th>
                                            <th className="px-4 py-3 text-right">Price</th>
                                            <th className="px-4 py-3 text-center">Qty</th>
                                            <th className="px-4 py-3 text-right">Discount</th>
                                            <th className="px-4 py-3 text-right">Total</th>
                                        </tr>
                                    </thead>
                                    <tbody className={`divide-y ${theme.border}`}>
                                        {sale.saleDetails.map((item, index) => (
                                            <tr key={index} className={theme.rowHover}>
                                                <td className={`px-4 py-3 ${theme.text}`}>
                                                    <div className="font-medium">{item.productName}</div>
                                                </td>
                                                <td className={`px-4 py-3 text-right ${theme.text}`}>
                                                    {formatCurrency(item.unitPrice)}
                                                </td>
                                                <td className={`px-4 py-3 text-center ${theme.text}`}>
                                                    {item.quantity}
                                                </td>
                                                <td className={`px-4 py-3 text-right text-red-500`}>
                                                    {(item.discount || 0) > 0 ? `-${formatCurrency(item.discount || 0)}` : '-'}
                                                </td>
                                                <td className={`px-4 py-3 text-right font-medium ${theme.text}`}>
                                                    {formatCurrency((item.unitPrice * item.quantity) - (item.discount || 0))}
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                    <tfoot className={`bg-gray-50 dark:bg-gray-800 border-t ${theme.border}`}>
                                        <tr>
                                            <td colSpan={4} className={`px-4 py-2 text-right ${theme.textSecondary}`}>Subtotal:</td>
                                            <td className={`px-4 py-2 text-right font-medium ${theme.text}`}>
                                                {formatCurrency(sale.subTotal || 0)}
                                            </td>
                                        </tr>
                                        {(sale.discount || 0) > 0 && (
                                            <tr>
                                                <td colSpan={4} className={`px-4 py-2 text-right text-red-500`}>Invoice Discount:</td>
                                                <td className={`px-4 py-2 text-right text-red-500 font-medium`}>
                                                    -{formatCurrency(sale.discount || 0)}
                                                </td>
                                            </tr>
                                        )}
                                        <tr>
                                            <td colSpan={4} className={`px-4 py-2 text-right font-bold ${theme.text} text-lg`}>Total:</td>
                                            <td className={`px-4 py-2 text-right font-bold text-lg text-blue-600`}>
                                                {formatCurrency(sale.totalAmount)}
                                            </td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>

                        {/* Payments */}
                        <div>
                            <h3 className={`text-sm font-semibold mb-3 ${theme.text} flex items-center gap-2`}>
                                <CreditCard size={16} /> Payment History
                            </h3>
                            {sale.payments && sale.payments.length > 0 ? (
                                <div className={`border rounded-lg overflow-hidden ${theme.border}`}>
                                    <table className="w-full text-sm">
                                        <thead className={`bg-gray-50 dark:bg-gray-800 border-b ${theme.border}`}>
                                            <tr className={theme.textSecondary}>
                                                <th className="px-4 py-3 text-left">Date</th>
                                                <th className="px-4 py-3 text-left">Method</th>
                                                <th className="px-4 py-3 text-left">Reference</th>
                                                <th className="px-4 py-3 text-right">Amount</th>
                                            </tr>
                                        </thead>
                                        <tbody className={`divide-y ${theme.border}`}>
                                            {sale.payments.map((payment, index) => (
                                                <tr key={index} className={theme.rowHover}>
                                                    <td className={`px-4 py-3 ${theme.text}`}>{formatDate(payment.paymentDate || '')}</td>
                                                    <td className={`px-4 py-3 ${theme.text}`}>{payment.paymentMethod}</td>
                                                    <td className={`px-4 py-3 ${theme.textSecondary}`}>{payment.referenceNo || '-'}</td>
                                                    <td className={`px-4 py-3 text-right font-medium text-green-600`}>
                                                        {formatCurrency(payment.amount)}
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            ) : (
                                <p className={`text-sm ${theme.textSecondary} italic`}>No payments recorded used.</p>
                            )}
                        </div>

                        {/* Payment Summary */}
                        <div className={`grid grid-cols-1 md:grid-cols-3 gap-4 mt-6`}>
                            <div className={`p-4 rounded-lg bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800`}>
                                <p className="text-xs text-green-600 dark:text-green-400 mb-1">Total Paid</p>
                                <p className="text-xl font-bold text-green-700 dark:text-green-300">{formatCurrency(totalPaid)}</p>
                            </div>
                            <div className={`p-4 rounded-lg bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800`}>
                                <p className="text-xs text-red-600 dark:text-red-400 mb-1">Balance Due</p>
                                <p className="text-xl font-bold text-red-700 dark:text-red-300">{formatCurrency(balanceDue)}</p>
                            </div>
                            <div className={`p-4 rounded-lg bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800`}>
                                <p className="text-xs text-blue-600 dark:text-blue-400 mb-1">Total Profit</p>
                                <p className="text-xl font-bold text-blue-700 dark:text-blue-300">{formatCurrency(sale.totalProfit || 0)}</p>
                            </div>
                        </div>

                    </div>
                </motion.div>
            </div>
        </AnimatePresence>
    );
}
