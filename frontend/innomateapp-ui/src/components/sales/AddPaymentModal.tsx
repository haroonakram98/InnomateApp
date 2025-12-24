import React, { useState, useEffect } from "react";
import Modal from "@/components/ui/Modal.js";
import Button from "@/components/ui/Button.js";
import Input from "@/components/ui/Input.js";
import Label from "@/components/ui/Label.js";
import { FormSelect } from "@/components/ui/Select.js";
import { addPaymentToSale } from "@/api/saleApi.js";
import { SaleDTO } from "@/types/sale.js";
import { useToastStore } from "@/store/useToastStore.js";

interface AddPaymentModalProps {
    isOpen: boolean;
    onClose: () => void;
    sale: SaleDTO | null;
    onSuccess: (updatedSale: SaleDTO) => void;
}

const AddPaymentModal: React.FC<AddPaymentModalProps> = ({
    isOpen,
    onClose,
    sale,
    onSuccess,
}) => {
    const [amount, setAmount] = useState<number>(0);
    const [method, setMethod] = useState<string>("Cash");
    const [reference, setReference] = useState<string>("");
    const [loading, setLoading] = useState(false);
    const toast = useToastStore(state => state.push);

    useEffect(() => {
        if (sale) {
            setAmount(sale.balanceAmount);
            setMethod("Cash");
            setReference("");
        }
    }, [sale]);

    const handleSubmit = async () => {
        if (!sale) return;

        if (amount <= 0) {
            toast("Amount must be greater than 0", "error");
            return;
        }

        if (amount > sale.balanceAmount) {
            toast(`Amount cannot exceed balance of ${sale.balanceAmount}`, "error");
            return;
        }

        setLoading(true);
        try {
            const updatedSale = await addPaymentToSale(sale.saleId, {
                amount,
                paymentMethod: method,
                referenceNo: reference || undefined,
            });
            toast("Payment added successfully", "success");
            onSuccess(updatedSale);
            onClose();
        } catch (error: any) {
            console.error("Error adding payment:", error);
            toast(error.response?.data?.error || "Failed to add payment", "error");
        } finally {
            setLoading(false);
        }
    };

    if (!sale) return null;

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={`Add Payment - INV #${sale.invoiceNo || sale.saleId}`}
            size="md"
        >
            <div className="space-y-4 py-2">
                <div className="bg-slate-50 dark:bg-slate-800/50 p-4 rounded-xl border border-slate-100 dark:border-slate-700 mb-6">
                    <div className="flex justify-between items-center mb-1">
                        <span className="text-slate-500 text-sm">Total Amount</span>
                        <span className="font-semibold text-slate-900 dark:text-white">Rs. {sale.totalAmount.toLocaleString()}</span>
                    </div>
                    <div className="flex justify-between items-center mb-1">
                        <span className="text-slate-500 text-sm">Paid Amount</span>
                        <span className="font-semibold text-emerald-600">Rs. {sale.paidAmount.toLocaleString()}</span>
                    </div>
                    <div className="h-px bg-slate-200 dark:bg-slate-700 my-2" />
                    <div className="flex justify-between items-center">
                        <span className="text-slate-900 dark:text-slate-200 font-bold">Remaining Balance</span>
                        <span className="font-bold text-rose-500 text-lg">Rs. {sale.balanceAmount.toLocaleString()}</span>
                    </div>
                </div>

                <div className="space-y-4">
                    <div className="grid gap-2">
                        <Label htmlFor="amount">Payment Amount</Label>
                        <Input
                            id="amount"
                            type="number"
                            value={amount}
                            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setAmount(Number(e.target.value))}
                            placeholder="Enter amount"
                            autoFocus
                        />
                    </div>

                    <FormSelect
                        label="Payment Method"
                        options={[
                            { value: "Cash", label: "Cash" },
                            { value: "Card", label: "Card" },
                            { value: "Bank Transfer", label: "Bank Transfer" },
                            { value: "Cheque", label: "Cheque" },
                        ]}
                        value={method}
                        onValueChange={(val) => setMethod(String(val))}
                    />

                    <div className="grid gap-2">
                        <Label htmlFor="ref">Reference No (Optional)</Label>
                        <Input
                            id="ref"
                            type="text"
                            value={reference}
                            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setReference(e.target.value)}
                            placeholder="e.g. Transaction ID, Check No"
                        />
                    </div>
                </div>

                <div className="flex justify-end gap-3 mt-8">
                    <Button variant="outline" onClick={onClose} disabled={loading}>
                        Cancel
                    </Button>
                    <Button onClick={handleSubmit} disabled={loading}>
                        {loading ? "Processing..." : "Submit Payment"}
                    </Button>
                </div>
            </div>
        </Modal>
    );
};

export default AddPaymentModal;
