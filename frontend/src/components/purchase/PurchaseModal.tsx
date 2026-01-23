import React, { useState, useEffect } from 'react';
import {
    X,
    Plus,
    Trash2,
    Package,
    Calendar,
    Tag,
    DollarSign,
    FileText,
    AlertCircle
} from 'lucide-react';
import { useTheme } from '@/hooks/useTheme.js';
import { useSuppliers } from '@/store/useSupplierStore.js';
import { useProducts } from '@/store/useProductStore.js';
import { usePurchaseActions } from '@/store/usePurchaseStore.js';
import { CreatePurchaseDTO, CreatePurchaseDetailDTO } from '@/types/purchase.js';
import { useAuthStore } from '@/store/useAuthStore.js';

interface PurchaseModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function PurchaseModal({ isOpen, onClose }: PurchaseModalProps) {
    const { isDark } = useTheme();
    const suppliers = useSuppliers().filter(s => s.isActive);
    const products = useProducts().filter(p => p.isActive);
    const { createPurchase, getNextPurchaseNumber, getNextBatchNumber } = usePurchaseActions();
    const { user } = useAuthStore();

    const [formData, setFormData] = useState<CreatePurchaseDTO>({
        supplierId: 0,
        purchaseDate: new Date().toISOString().split('T')[0],
        notes: '',
        invoiceNo: '',
        createdBy: user?.id || 1,
        purchaseDetails: []
    });

    const [newDetail, setNewDetail] = useState<CreatePurchaseDetailDTO>({
        productId: 0,
        quantity: 0,
        unitCost: 0,
        batchNo: '',
        expiryDate: ''
    });

    // Load initial data
    useEffect(() => {
        if (isOpen) {
            const loadNextNo = async () => {
                const no = await getNextPurchaseNumber();
                if (no) setFormData(prev => ({ ...prev, invoiceNo: no }));
            };
            loadNextNo();
        }
    }, [isOpen]);

    const handleAddDetail = async () => {
        if (!newDetail.productId || newDetail.quantity <= 0 || newDetail.unitCost <= 0) return;

        setFormData(prev => ({
            ...prev,
            purchaseDetails: [...prev.purchaseDetails, { ...newDetail }]
        }));

        setNewDetail({
            productId: 0,
            quantity: 0,
            unitCost: 0,
            batchNo: '',
            expiryDate: ''
        });
    };

    const handleRemoveDetail = (index: number) => {
        setFormData(prev => ({
            ...prev,
            purchaseDetails: prev.purchaseDetails.filter((_, i) => i !== index)
        }));
    };

    const handleSubmit = async () => {
        try {
            await createPurchase(formData);
            onClose();
        } catch (error) {
            console.error(error);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-[110] flex items-center justify-center p-6">
            <div className="absolute inset-0 bg-black/60 backdrop-blur-md" onClick={onClose} />

            <div className={`relative w-full max-w-5xl max-h-[90vh] rounded-[3rem] shadow-2xl overflow-hidden flex flex-col animate-in fade-in zoom-in duration-300 ${isDark ? 'bg-[#0f172a] border border-gray-800' : 'bg-white border border-gray-100'
                }`}>

                {/* Header */}
                <div className="p-10 border-b flex items-center justify-between bg-indigo-600">
                    <div className="flex items-center gap-6">
                        <div className="h-16 w-16 bg-white/20 backdrop-blur-xl rounded-2xl flex items-center justify-center text-white">
                            <Plus size={32} strokeWidth={3} />
                        </div>
                        <div>
                            <h2 className="text-3xl font-black text-white">Create Purchase</h2>
                            <p className="text-white/70 font-bold uppercase tracking-widest text-xs mt-1">Stock Replenishment Entry</p>
                        </div>
                    </div>
                    <button onClick={onClose} className="p-4 bg-white/10 hover:bg-white/20 text-white rounded-2xl transition-all">
                        <X size={24} />
                    </button>
                </div>

                {/* Form Body */}
                <div className="flex-1 overflow-y-auto p-10 space-y-10">

                    {/* Main Info Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                        <div className="space-y-2">
                            <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Supplier Account</label>
                            <div className="relative">
                                <select
                                    value={formData.supplierId}
                                    onChange={(e) => setFormData({ ...formData, supplierId: Number(e.target.value) })}
                                    className={`w-full pl-5 pr-10 py-4 rounded-2xl font-bold appearance-none transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500' : 'bg-gray-50 border-gray-100 focus:border-indigo-500'
                                        }`}
                                >
                                    <option value={0}>Select Supplier</option>
                                    {suppliers.map(s => <option key={s.supplierId} value={s.supplierId}>{s.name}</option>)}
                                </select>
                            </div>
                        </div>

                        <div className="space-y-2">
                            <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Invoice / Ref No</label>
                            <input
                                type="text"
                                value={formData.invoiceNo}
                                onChange={(e) => setFormData({ ...formData, invoiceNo: e.target.value })}
                                className={`w-full px-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500' : 'bg-gray-50 border-gray-100 focus:border-indigo-500'
                                    }`}
                            />
                        </div>

                        <div className="space-y-2">
                            <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Transaction Date</label>
                            <input
                                type="date"
                                value={formData.purchaseDate}
                                onChange={(e) => setFormData({ ...formData, purchaseDate: e.target.value })}
                                className={`w-full px-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500' : 'bg-gray-50 border-gray-100 focus:border-indigo-500'
                                    }`}
                            />
                        </div>
                    </div>

                    {/* Line Items Entry Area */}
                    <div className={`p-8 rounded-[2.5rem] ${isDark ? 'bg-indigo-500/5 border border-indigo-500/10' : 'bg-indigo-50/50 border border-indigo-100'}`}>
                        <h4 className="text-xs font-black text-indigo-600 uppercase tracking-widest mb-6 px-2">Log Line Items</h4>

                        <div className="grid grid-cols-1 md:grid-cols-6 gap-4 mb-8">
                            <div className="md:col-span-2">
                                <select
                                    value={newDetail.productId}
                                    onChange={async (e) => {
                                        const id = Number(e.target.value);
                                        const bNo = id > 0 ? await getNextBatchNumber() : '';
                                        setNewDetail({ ...newDetail, productId: id, batchNo: bNo || '' });
                                    }}
                                    className={`w-full px-5 py-3 rounded-xl font-bold outline-none ${isDark ? 'bg-gray-800' : 'bg-white shadow-sm'}`}
                                >
                                    <option value={0}>Choose Product</option>
                                    {products.map(p => <option key={p.productId} value={p.productId}>{p.name}</option>)}
                                </select>
                            </div>
                            <input
                                type="number"
                                placeholder="Qty"
                                value={newDetail.quantity || ''}
                                onChange={(e) => setNewDetail({ ...newDetail, quantity: Number(e.target.value) })}
                                className={`px-5 py-3 rounded-xl font-bold outline-none ${isDark ? 'bg-gray-800' : 'bg-white shadow-sm'}`}
                            />
                            <input
                                type="number"
                                placeholder="Cost"
                                value={newDetail.unitCost || ''}
                                onChange={(e) => setNewDetail({ ...newDetail, unitCost: Number(e.target.value) })}
                                className={`px-5 py-3 rounded-xl font-bold outline-none ${isDark ? 'bg-gray-800' : 'bg-white shadow-sm'}`}
                            />
                            <input
                                type="text"
                                placeholder="Batch"
                                value={newDetail.batchNo}
                                onChange={(e) => setNewDetail({ ...newDetail, batchNo: e.target.value })}
                                className={`px-5 py-3 rounded-xl font-bold outline-none ${isDark ? 'bg-gray-800' : 'bg-white shadow-sm'}`}
                            />
                            <button
                                onClick={handleAddDetail}
                                className="bg-indigo-600 text-white rounded-xl font-black transition-all hover:bg-indigo-700 active:scale-95"
                            >
                                Insert
                            </button>
                        </div>

                        {/* Added Items List */}
                        <div className="space-y-3">
                            {formData.purchaseDetails.map((item, idx) => (
                                <div key={idx} className={`flex items-center justify-between p-4 rounded-2xl ${isDark ? 'bg-gray-800/40' : 'bg-white shadow-sm'}`}>
                                    <div className="flex items-center gap-4">
                                        <div className="h-10 w-10 rounded-xl bg-indigo-500/10 text-indigo-500 flex items-center justify-center font-black">{idx + 1}</div>
                                        <div>
                                            <div className="font-bold">{products.find(p => p.productId === item.productId)?.name}</div>
                                            <div className="text-[10px] font-black text-gray-400 uppercase">Batch: {item.batchNo}</div>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-8">
                                        <div className="text-right">
                                            <div className="font-black">{item.quantity} × ${item.unitCost}</div>
                                            <div className="text-[10px] font-black text-indigo-500 uppercase">${(item.quantity * item.unitCost).toFixed(2)}</div>
                                        </div>
                                        <button onClick={() => handleRemoveDetail(idx)} className="p-2 text-rose-500 hover:bg-rose-500/10 rounded-lg">
                                            <Trash2 size={18} />
                                        </button>
                                    </div>
                                </div>
                            ))}

                            {formData.purchaseDetails.length > 0 && (
                                <div className="flex justify-between items-center pt-6 px-4">
                                    <div className="text-xs font-black text-gray-400 uppercase tracking-widest">Total Valuation</div>
                                    <div className="text-3xl font-black text-indigo-600">
                                        ${formData.purchaseDetails.reduce((acc, i) => acc + (i.quantity * i.unitCost), 0).toLocaleString(undefined, { minimumFractionDigits: 2 })}
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Notes Area */}
                    <div className="space-y-2">
                        <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Logistical Notes</label>
                        <textarea
                            value={formData.notes}
                            onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                            rows={3}
                            placeholder="Internal tracking notes, expected delivery details, etc."
                            className={`w-full px-6 py-4 rounded-[2rem] font-medium transition-all outline-none border-2 resize-none ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500' : 'bg-gray-50 border-gray-100 focus:border-indigo-500'
                                }`}
                        />
                    </div>
                </div>

                {/* Action Footer */}
                <div className="p-10 border-t flex justify-end gap-4">
                    <button onClick={onClose} className={`px-10 py-4 rounded-3xl font-bold transition-all ${isDark ? 'bg-gray-800 hover:bg-gray-700' : 'bg-gray-100 hover:bg-gray-200'}`}>
                        Discard
                    </button>
                    <button
                        onClick={handleSubmit}
                        disabled={formData.purchaseDetails.length === 0 || formData.supplierId === 0}
                        className="px-12 py-4 bg-indigo-600 text-white rounded-3xl font-black shadow-xl shadow-indigo-600/20 hover:scale-[1.02] active:scale-[0.98] transition-all disabled:opacity-50 disabled:scale-100"
                    >
                        Authorize Purchase
                    </button>
                </div>
            </div>
        </div>
    );
}
