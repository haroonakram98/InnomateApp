import React, { useState, useEffect } from 'react';
import {
    X,
    Plus,
    Package,
    Tag,
    DollarSign,
    Layers,
    ShieldCheck,
    LayoutGrid
} from 'lucide-react';
import { useTheme } from '@/hooks/useTheme.js';
import { useProductActions } from '@/store/useProductStore.js';
import { useCategories } from '@/store/useCategoryStore.js';
import { ProductDTO, CreateProductDto, UpdateProductDto } from '@/types/product.js';

interface ProductModalProps {
    isOpen: boolean;
    onClose: () => void;
    product?: ProductDTO | null;
}

export default function ProductModal({ isOpen, onClose, product }: ProductModalProps) {
    const { isDark } = useTheme();
    const { createProduct, updateProduct } = useProductActions();
    const categories = useCategories();

    const [formData, setFormData] = useState<CreateProductDto>({
        name: '',
        categoryId: 0,
        sku: '',
        reorderLevel: 0,
        isActive: true,
        defaultSalePrice: 0
    });

    useEffect(() => {
        if (product) {
            setFormData({
                name: product.name,
                categoryId: product.categoryId,
                sku: product.sku || '',
                reorderLevel: product.reorderLevel,
                isActive: product.isActive,
                defaultSalePrice: product.defaultSalePrice
            });
        } else {
            setFormData({
                name: '',
                categoryId: 0,
                sku: '',
                reorderLevel: 5,
                isActive: true,
                defaultSalePrice: 0
            });
        }
    }, [product, isOpen]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (product) {
                await updateProduct({ ...formData, productId: product.productId } as UpdateProductDto);
            } else {
                await createProduct(formData);
            }
            onClose();
        } catch (error) {
            console.error(error);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-[110] flex items-center justify-center p-6">
            <div className="absolute inset-0 bg-black/60 backdrop-blur-md" onClick={onClose} />

            <div className={`relative w-full max-w-2xl rounded-[3rem] shadow-2xl overflow-hidden flex flex-col animate-in fade-in zoom-in duration-300 ${isDark ? 'bg-[#0f172a] border border-gray-800' : 'bg-white border border-gray-100'
                }`}>

                {/* Header */}
                <div className="p-10 border-b flex items-center justify-between bg-indigo-600 font-sans">
                    <div className="flex items-center gap-6">
                        <div className="h-16 w-16 bg-white/20 backdrop-blur-xl rounded-2xl flex items-center justify-center text-white">
                            <Package size={32} strokeWidth={2.5} />
                        </div>
                        <div>
                            <h2 className="text-3xl font-black text-white">{product ? 'Edit Product' : 'Add Product'}</h2>
                            <p className="text-white/70 font-bold uppercase tracking-widest text-xs mt-1">Catalog Management Entry</p>
                        </div>
                    </div>
                    <button onClick={onClose} className="p-4 bg-white/10 hover:bg-white/20 text-white rounded-2xl transition-all">
                        <X size={24} />
                    </button>
                </div>

                {/* Form Body */}
                <form onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-10 space-y-8">

                    <div className="space-y-6">
                        {/* Name Field */}
                        <div className="space-y-2">
                            <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Product Identification</label>
                            <div className="relative group">
                                <Tag className={`absolute left-5 top-1/2 -translate-y-1/2 transition-colors ${isDark ? 'text-gray-600 group-focus-within:text-indigo-400' : 'text-gray-400 group-focus-within:text-indigo-500'}`} size={20} />
                                <input
                                    required
                                    type="text"
                                    placeholder="Official Product Name"
                                    value={formData.name}
                                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                    className={`w-full pl-14 pr-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500 text-white' : 'bg-gray-50 border-gray-100 focus:border-indigo-500 text-gray-900'
                                        }`}
                                />
                            </div>
                        </div>

                        {/* SKU and Category Row */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div className="space-y-2">
                                <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">SKU / Serial No</label>
                                <input
                                    type="text"
                                    placeholder="Reference SKU"
                                    value={formData.sku}
                                    onChange={(e) => setFormData({ ...formData, sku: e.target.value })}
                                    className={`w-full px-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500 text-white' : 'bg-gray-50 border-gray-100 focus:border-indigo-500 text-gray-900'
                                        }`}
                                />
                            </div>
                            <div className="space-y-2">
                                <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Category Segment</label>
                                <select
                                    required
                                    value={formData.categoryId}
                                    onChange={(e) => setFormData({ ...formData, categoryId: Number(e.target.value) })}
                                    className={`w-full px-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 appearance-none ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-indigo-500 text-white' : 'bg-gray-50 border-gray-100 focus:border-indigo-500 text-gray-900'
                                        }`}
                                >
                                    <option value={0}>Select Segment</option>
                                    {categories.map(c => <option key={c.categoryId} value={c.categoryId}>{c.name}</option>)}
                                </select>
                            </div>
                        </div>

                        {/* Pricing and Logistics Row */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div className="space-y-2">
                                <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Standard Sales Price</label>
                                <div className="relative group">
                                    <DollarSign className={`absolute left-5 top-1/2 -translate-y-1/2 transition-colors ${isDark ? 'text-gray-600 group-focus-within:text-emerald-400' : 'text-gray-400 group-focus-within:text-emerald-500'}`} size={20} />
                                    <input
                                        required
                                        type="number"
                                        step="0.01"
                                        placeholder="0.00"
                                        value={formData.defaultSalePrice || ''}
                                        onChange={(e) => setFormData({ ...formData, defaultSalePrice: Number(e.target.value) })}
                                        className={`w-full pl-14 pr-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-emerald-500 text-white' : 'bg-gray-50 border-gray-100 focus:border-emerald-500 text-gray-900'
                                            }`}
                                    />
                                </div>
                            </div>
                            <div className="space-y-2">
                                <label className="text-[10px] font-black uppercase tracking-widest text-gray-400 ml-2">Critical Reorder Threshold</label>
                                <div className="relative group">
                                    <Layers className={`absolute left-5 top-1/2 -translate-y-1/2 transition-colors ${isDark ? 'text-gray-600 group-focus-within:text-amber-400' : 'text-gray-400 group-focus-within:text-amber-500'}`} size={20} />
                                    <input
                                        required
                                        type="number"
                                        placeholder="5"
                                        value={formData.reorderLevel || ''}
                                        onChange={(e) => setFormData({ ...formData, reorderLevel: Number(e.target.value) })}
                                        className={`w-full pl-14 pr-6 py-4 rounded-2xl font-bold transition-all outline-none border-2 ${isDark ? 'bg-gray-800/50 border-gray-700 focus:border-amber-500 text-white' : 'bg-gray-50 border-gray-100 focus:border-amber-500 text-gray-900'
                                            }`}
                                    />
                                </div>
                            </div>
                        </div>

                        {/* Status Toggle */}
                        <div className={`p-6 rounded-3xl border flex items-center justify-between ${isDark ? 'bg-indigo-500/5 border-indigo-500/10' : 'bg-indigo-50 border-indigo-100'}`}>
                            <div className="flex items-center gap-4">
                                <div className="p-3 bg-indigo-500 rounded-2xl text-white">
                                    <ShieldCheck size={20} />
                                </div>
                                <div>
                                    <div className="font-black text-sm">Product Status</div>
                                    <div className="text-[10px] font-black text-gray-400 uppercase">Available for trade operations</div>
                                </div>
                            </div>
                            <button
                                type="button"
                                onClick={() => setFormData({ ...formData, isActive: !formData.isActive })}
                                className={`relative inline-flex h-8 w-14 items-center rounded-full transition-colors duration-300 focus:outline-none ${formData.isActive ? 'bg-emerald-500' : 'bg-gray-400'}`}
                            >
                                <span className={`inline-block h-6 w-6 transform rounded-full bg-white transition-transform duration-300 ${formData.isActive ? 'translate-x-7' : 'translate-x-1'}`} />
                            </button>
                        </div>
                    </div>
                </form>

                {/* Action Footer */}
                <div className="p-10 border-t flex justify-end gap-4">
                    <button onClick={onClose} className={`px-10 py-4 rounded-3xl font-bold transition-all ${isDark ? 'bg-gray-800 hover:bg-gray-700' : 'bg-gray-100 hover:bg-gray-200'}`}>
                        Discard
                    </button>
                    <button
                        onClick={handleSubmit}
                        className="px-12 py-4 bg-indigo-600 text-white rounded-3xl font-black shadow-xl shadow-indigo-600/20 hover:scale-[1.02] active:scale-[0.98] transition-all"
                    >
                        {product ? 'Synchronize Data' : 'Authorize Addition'}
                    </button>
                </div>
            </div>
        </div>
    );
}
