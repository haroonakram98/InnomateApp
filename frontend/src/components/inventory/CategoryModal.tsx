import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { X, Save, RotateCcw } from 'lucide-react';
import { categorySchema, CategoryFormData } from '@/lib/schemas/categorySchema.js';
import { useCategoryStore, useCategoryActions, useCategoryValidationErrors } from '@/store/useCategoryStore.js';
import { useTheme } from '@/hooks/useTheme.js';
import { CategoryDTO } from '@/types/category.js';

interface CategoryModalProps {
    isOpen: boolean;
    onClose: () => void;
    category?: CategoryDTO | null;
}

const CategoryModal: React.FC<CategoryModalProps> = ({ isOpen, onClose, category }) => {
    const { isDark } = useTheme();
    const { createCategory, updateCategory, clearError } = useCategoryActions();
    const isLoading = useCategoryStore(state => state.isLoading);
    const validationErrors = useCategoryValidationErrors();

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
        setValue,
    } = useForm<CategoryFormData>({
        resolver: zodResolver(categorySchema),
        defaultValues: {
            name: '',
            description: '',
        },
    });

    useEffect(() => {
        if (category) {
            setValue('name', category.name);
            setValue('description', category.description || '');
        } else {
            reset({
                name: '',
                description: '',
            });
        }
        clearError();
    }, [category, setValue, reset, clearError, isOpen]);

    const onSubmit = async (data: CategoryFormData) => {
        try {
            if (category) {
                await updateCategory(category.categoryId, {
                    ...data,
                    categoryId: category.categoryId,
                });
            } else {
                await createCategory(data);
            }
            onClose();
        } catch (error) {
            // Error handling is managed by the store
        }
    };

    if (!isOpen) return null;

    const getFieldError = (fieldName: string) => {
        if (!validationErrors) return null;
        const lower = fieldName.toLowerCase();
        const capitalized = fieldName.charAt(0).toUpperCase() + fieldName.slice(1);

        const err =
            validationErrors[lower] ||
            validationErrors[capitalized] ||
            validationErrors[`CategoryDto.${capitalized}`] ||
            validationErrors[`CategoryDto.${lower}`];

        return err ? err[0] : null;
    };

    const inputClasses = `w-full px-4 py-2.5 rounded-xl border transition-all duration-200 outline-none focus:ring-2 ${isDark
        ? 'bg-gray-800 border-gray-700 text-white focus:ring-blue-500/40 focus:border-blue-500'
        : 'bg-white border-gray-200 text-gray-900 focus:ring-blue-500/20 focus:border-blue-500'
        }`;

    const labelClasses = `block text-sm font-medium mb-1.5 ${isDark ? 'text-gray-300' : 'text-gray-700'}`;
    const errorClasses = "mt-1.5 text-xs font-medium text-red-500 flex items-center gap-1 animate-in fade-in slide-in-from-top-1";

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-300">
            <div
                className={`w-full max-w-lg rounded-3xl shadow-2xl overflow-hidden animate-in zoom-in-95 duration-200 ${isDark ? 'bg-gray-900 border border-gray-800' : 'bg-white'
                    }`}
            >
                {/* Header */}
                <div className={`px-8 py-6 flex items-center justify-between border-b ${isDark ? 'border-gray-800' : 'border-gray-100'}`}>
                    <div>
                        <h2 className={`text-2xl font-bold ${isDark ? 'text-white' : 'text-gray-900'}`}>
                            {category ? 'Edit Category' : 'New Category'}
                        </h2>
                        <p className={`text-sm mt-1 ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>
                            {category ? 'Update the details of the category.' : 'Add a new product category to your system.'}
                        </p>
                    </div>
                    <button
                        onClick={onClose}
                        className={`p-2.5 rounded-xl transition-all duration-200 ${isDark ? 'bg-gray-800 text-gray-400 hover:text-white hover:bg-gray-700' : 'bg-gray-50 text-gray-500 hover:text-gray-900 hover:bg-gray-100'
                            }`}
                    >
                        <X size={20} />
                    </button>
                </div>

                <form onSubmit={handleSubmit(onSubmit)} className="p-8 space-y-6">
                    {/* Name */}
                    <div className="group">
                        <label className={labelClasses}>Category Name</label>
                        <div className="relative">
                            <input
                                {...register('name')}
                                type="text"
                                className={inputClasses}
                                placeholder="e.g. Electronics, Furniture"
                            />
                        </div>
                        {(errors.name || getFieldError('Name')) && (
                            <p className={errorClasses}>
                                {errors.name?.message || getFieldError('Name')}
                            </p>
                        )}
                    </div>

                    {/* Description */}
                    <div className="group">
                        <label className={labelClasses}>Description (Optional)</label>
                        <div className="relative">
                            <textarea
                                {...register('description')}
                                rows={4}
                                className={`${inputClasses} resize-none`}
                                placeholder="Provide more details about this category..."
                            />
                        </div>
                        {(errors.description || getFieldError('Description')) && (
                            <p className={errorClasses}>
                                {errors.description?.message || getFieldError('Description')}
                            </p>
                        )}
                    </div>

                    {/* Footer */}
                    <div className={`pt-6 flex gap-3 border-t ${isDark ? 'border-gray-800' : 'border-gray-100'}`}>
                        <button
                            type="button"
                            onClick={onClose}
                            className={`flex-1 px-6 py-3 rounded-xl font-semibold transition-all duration-200 flex items-center justify-center gap-2 ${isDark
                                ? 'bg-gray-800 text-gray-300 hover:bg-gray-700 hover:text-white'
                                : 'bg-gray-100 text-gray-600 hover:bg-gray-200 hover:text-gray-900'
                                }`}
                        >
                            <RotateCcw size={18} />
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={isLoading}
                            className="flex-[2] px-6 py-3 rounded-xl bg-blue-600 text-white font-semibold hover:bg-blue-700 active:scale-[0.98] transition-all duration-200 flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-blue-600/20"
                        >
                            {isLoading ? (
                                <div className="h-5 w-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                            ) : (
                                <>
                                    <Save size={18} />
                                    {category ? 'Save Changes' : 'Create Category'}
                                </>
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default CategoryModal;
