"use client"

import { useEffect } from "react"
import { useForm, useFieldArray, Controller } from "react-hook-form"
import { z } from "zod"
import { zodResolver } from "@hookform/resolvers/zod"
import Button from "@/components/ui/Button.js"
import Input from "@/components/ui/Input.js"
import { Card } from "@/components/ui/Card.js"
import { motion, AnimatePresence } from "framer-motion"
import { Trash2, Plus, X } from "lucide-react"
import type { SaleDTO } from "@/types/sale.js"
import { FormSelect } from "@/components/ui/Select.js"
import { useCustomerStore } from "@/store/usecustomerStore.js"
import { useProductStore } from "@/store/useProductStore.js"
import { useSaleStore } from "@/store/useSaleStore.js"

//
// -------------------- Schema --------------------
//
const saleSchema = z.object({
  customerId: z.number().int().min(1, "Customer is required"),
  invoiceNo: z.string().optional(),
  saleDetails: z
    .array(
      z.object({
        productId: z.number().int().min(1, "Product required"),
        quantity: z.number().min(0.01, "Quantity must be greater than 0"),
        unitPrice: z.number().min(0, "Price must be >= 0"),
        purchaseDetailId: z.number().int().nullable().optional(),
      }),
    )
    .min(1, "Add at least one product"),
  payments: z
    .array(
      z.object({
        paymentMethod: z.enum(["Cash", "Card", "Bank Transfer", "Mobile Payment"], {
          errorMap: () => ({ message: "Select a valid payment method" })
        }),
        amount: z.number().min(0.01, "Amount must be greater than 0"),
        referenceNo: z.string().nullable().optional(),
      }),
    )
    .min(0)
    .default([]),
})

export type FormValues = z.infer<typeof saleSchema>

//
// -------------------- Props Interface --------------------
//
interface SaleFormProps {
  saleId?: number | null
  initial?: Partial<SaleDTO>
  onClose: (refresh?: boolean) => void
}

//
// -------------------- Component --------------------
//
export default function SaleForm({ saleId, initial, onClose }: SaleFormProps) {
  const { customers, fetchCustomers } = useCustomerStore()
  const { products, fetchProducts } = useProductStore()
  const { createSale, updateSale } = useSaleStore()

  const {
    control,
    register,
    handleSubmit,
    watch,
    reset,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(saleSchema),
    mode: "onBlur",
    defaultValues: {
      customerId: initial?.customerId ?? 0,
      invoiceNo: initial?.invoiceNo ?? "",
      saleDetails: initial?.saleDetails?.length
        ? initial.saleDetails.map((sd) => ({
            productId: sd.productId ?? 0,
            quantity: sd.quantity ?? 1,
            unitPrice: sd.unitPrice ?? 0,
            purchaseDetailId: sd.purchaseDetailId ?? null,
          }))
        : [{ productId: 0, quantity: 1, unitPrice: 0, purchaseDetailId: null }],
      payments: initial?.payments?.length
        ? initial.payments.map((p) => ({
            paymentMethod: p.paymentMethod ?? "",
            amount: p.amount ?? 0,
            referenceNo: p.referenceNo ?? null,
          }))
        : [],
    },
  })

  const { fields, append, remove } = useFieldArray({
    control,
    name: "saleDetails",
  })

  const {
    fields: paymentFields,
    append: appendPayment,
    remove: removePayment,
  } = useFieldArray({
    control,
    name: "payments",
  })

  const saleDetails = watch("saleDetails") || []
  const payments = watch("payments") || []
  
  const subtotal = saleDetails.reduce((sum, item) => {
    const qty = Number(item.quantity) || 0
    const price = Number(item.unitPrice) || 0
    return sum + (qty * price)
  }, 0)
  
  const totalPaid = payments.reduce((sum, p) => sum + (Number(p.amount) || 0), 0)
  const balance = subtotal - totalPaid

  // Fetch data on mount
  useEffect(() => {
    fetchCustomers()
    fetchProducts()
  }, [fetchCustomers, fetchProducts])

  // Auto-fill product price when product is selected
  const handleProductChange = (index: number, productId: number) => {
    const product = products.find(p => p.productId === productId)
    if (product) {
      setValue(`saleDetails.${index}.unitPrice`, product.defaultSalePrice || 0)
    }
  }

  //
  // -------------------- Submit Handler --------------------
  //
  const onSubmit = async (data: FormValues) => {
    try {
      const payload: SaleDTO = {
        ...data,
        totalAmount: subtotal,
      }

      if (saleId) {
        await updateSale(saleId, payload)
      } else {
        await createSale(payload)
      }

      onClose(true)
    } catch (err) {
      console.error("Error saving sale:", err)
      alert("Failed to save sale. Please try again.")
    }
  }

  // Customer options
  const customerOptions = [
    { value: 0, label: "Select Customer" },
    ...customers.map(c => ({ value: c.customerId, label: c.name }))
  ]

  // Product options
  const productOptions = [
    { value: 0, label: "Select Product" },
    ...products.map(p => ({ value: p.productId, label: `${p.name} - Rs. ${p.defaultSalePrice}` }))
  ]

  // Payment method options
  const paymentMethodOptions = [
    { value: "Cash", label: "Cash" },
    { value: "Card", label: "Card" },
    { value: "Bank Transfer", label: "Bank Transfer" },
    { value: "Mobile Payment", label: "Mobile Payment" },
  ]

  //
  // -------------------- JSX --------------------
  //
  return (
    <motion.div 
      initial={{ opacity: 0, scale: 0.95 }} 
      animate={{ opacity: 1, scale: 1 }} 
      exit={{ opacity: 0, scale: 0.95 }}
      transition={{ duration: 0.2 }}
      className="w-full h-full overflow-auto"
    >
      <div className="p-6 max-w-5xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
            {saleId ? "✏️ Edit Sale" : "🧾 Create New Sale"}
          </h2>
          <button
            type="button"
            onClick={() => onClose(false)}
            className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Customer & Invoice */}
          <Card className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormSelect
                label="Customer *"
                placeholder="Select Customer"
                options={customerOptions}
                value={watch("customerId")}
                onValueChange={(value) => setValue("customerId", value as number)}
                error={errors.customerId?.message}
              />
              <Input 
                label="Invoice No"
                {...register("invoiceNo")} 
                placeholder="Auto-generated if empty" 
              />
            </div>
          </Card>

          {/* Sale Details */}
          <Card className="p-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">Products</h3>
              <Button
                type="button"
                size="sm"
                onClick={() => append({ productId: 0, quantity: 1, unitPrice: 0, purchaseDetailId: null })}
              >
                <Plus className="w-4 h-4 mr-1" />
                Add Product
              </Button>
            </div>

            <div className="space-y-3">
              <AnimatePresence mode="popLayout">
                {fields.map((field, index) => (
                  <motion.div
                    key={field.id}
                    initial={{ opacity: 0, height: 0 }}
                    animate={{ opacity: 1, height: "auto" }}
                    exit={{ opacity: 0, height: 0 }}
                    transition={{ duration: 0.2 }}
                  >
                    <div className="grid grid-cols-12 gap-3 items-start p-3 bg-gray-50 dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
                      {/* Product Select */}
                      <div className="col-span-12 md:col-span-5">
                        <label className="text-xs font-medium text-gray-600 dark:text-gray-400 mb-1 block">
                          Product
                        </label>
                        <Controller
                          name={`saleDetails.${index}.productId`}
                          control={control}
                          render={({ field }) => (
                            <FormSelect
                              {...field}
                              options={productOptions}
                              onValueChange={(value) => {
                                field.onChange(value)
                                handleProductChange(index, value as number)
                              }}
                              error={errors.saleDetails?.[index]?.productId?.message}
                            />
                          )}
                        />
                      </div>

                      {/* Quantity */}
                      <div className="col-span-6 md:col-span-3">
                        <label className="text-xs font-medium text-gray-600 dark:text-gray-400 mb-1 block">
                          Quantity
                        </label>
                        <Input
                          type="number"
                          step="0.01"
                          min="0.01"
                          {...register(`saleDetails.${index}.quantity`, { valueAsNumber: true })}
                          error={errors.saleDetails?.[index]?.quantity?.message}
                          placeholder="Qty"
                        />
                      </div>

                      {/* Unit Price */}
                      <div className="col-span-6 md:col-span-3">
                        <label className="text-xs font-medium text-gray-600 dark:text-gray-400 mb-1 block">
                          Price
                        </label>
                        <Input
                          type="number"
                          step="0.01"
                          min="0"
                          {...register(`saleDetails.${index}.unitPrice`, { valueAsNumber: true })}
                          error={errors.saleDetails?.[index]?.unitPrice?.message}
                          placeholder="Price"
                        />
                      </div>

                      {/* Remove Button */}
                      <div className="col-span-12 md:col-span-1 flex items-end justify-end md:justify-center">
                        <Button
                          type="button"
                          variant="danger"
                          size="sm"
                          onClick={() => remove(index)}
                          disabled={fields.length === 1}
                          className="mt-1 md:mt-0"
                        >
                          <Trash2 className="w-4 h-4" />
                        </Button>
                      </div>

                      {/* Subtotal Display */}
                      <div className="col-span-12 text-right text-sm font-medium text-gray-600 dark:text-gray-400">
                        Subtotal: Rs. {((watch(`saleDetails.${index}.quantity`) || 0) * (watch(`saleDetails.${index}.unitPrice`) || 0)).toFixed(2)}
                      </div>
                    </div>
                  </motion.div>
                ))}
              </AnimatePresence>
            </div>
            
            {errors.saleDetails && typeof errors.saleDetails.message === 'string' && (
              <p className="text-red-500 text-sm mt-2">{errors.saleDetails.message}</p>
            )}
          </Card>

          {/* Payments */}
          <Card className="p-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">Payments</h3>
              <Button
                type="button"
                size="sm"
                variant="secondary"
                onClick={() => appendPayment({ 
                  paymentMethod: "Cash", 
                  amount: balance > 0 ? balance : 0, 
                  referenceNo: "" 
                })}
              >
                <Plus className="w-4 h-4 mr-1" />
                Add Payment
              </Button>
            </div>

            {paymentFields.length > 0 ? (
              <div className="space-y-3">
                <AnimatePresence mode="popLayout">
                  {paymentFields.map((field, index) => (
                    <motion.div
                      key={field.id}
                      initial={{ opacity: 0, height: 0 }}
                      animate={{ opacity: 1, height: "auto" }}
                      exit={{ opacity: 0, height: 0 }}
                      transition={{ duration: 0.2 }}
                    >
                      <div className="grid grid-cols-12 gap-3 items-start p-3 bg-gray-50 dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700">
                        <div className="col-span-12 md:col-span-4">
                          <label className="text-xs font-medium text-gray-600 dark:text-gray-400 mb-1 block">
                            Method
                          </label>
                          <Controller
                            name={`payments.${index}.paymentMethod`}
                            control={control}
                            render={({ field }) => (
                              <FormSelect
                                {...field}
                                options={paymentMethodOptions}
                                onValueChange={(value) => field.onChange(value)}
                                error={errors.payments?.[index]?.paymentMethod?.message}
                              />
                            )}
                          />
                        </div>
                        <div className="col-span-6 md:col-span-3">
                          <label className="text-xs font-medium text-gray-600 dark:text-gray-400 mb-1 block">
                            Amount
                          </label>
                          <Input
                            type="number"
                            step="0.01"
                            min="0.01"
                            {...register(`payments.${index}.amount`, { valueAsNumber: true })}
                            error={errors.payments?.[index]?.amount?.message}
                            placeholder="Amount"
                          />
                        </div>
                        <div className="col-span-6 md:col-span-4">
                          <label className="text-xs font-medium text-gray-600 dark:text-gray-400 mb-1 block">
                            Reference
                          </label>
                          <Input
                            {...register(`payments.${index}.referenceNo`)}
                            placeholder="Optional"
                          />
                        </div>
                        <div className="col-span-12 md:col-span-1 flex items-end justify-end md:justify-center">
                          <Button
                            type="button"
                            variant="danger"
                            size="sm"
                            onClick={() => removePayment(index)}
                            className="mt-1 md:mt-0"
                          >
                            <Trash2 className="w-4 h-4" />
                          </Button>
                        </div>
                      </div>
                    </motion.div>
                  ))}
                </AnimatePresence>
              </div>
            ) : (
              <p className="text-gray-500 text-sm text-center py-4">No payments added yet</p>
            )}
          </Card>

          {/* Summary */}
          <Card className="p-4 bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-800">
            <div className="space-y-2">
              <div className="flex justify-between items-center text-lg">
                <span className="font-medium">Subtotal:</span>
                <span className="font-bold">Rs. {subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between items-center text-lg text-green-600 dark:text-green-400">
                <span className="font-medium">Paid:</span>
                <span className="font-bold">Rs. {totalPaid.toFixed(2)}</span>
              </div>
              <div className="border-t border-blue-300 dark:border-blue-700 pt-2">
                <div className={`flex justify-between items-center text-xl ${balance > 0 ? 'text-red-600 dark:text-red-400' : 'text-gray-900 dark:text-gray-100'}`}>
                  <span className="font-semibold">Balance:</span>
                  <span className="font-bold">Rs. {balance.toFixed(2)}</span>
                </div>
              </div>
            </div>
          </Card>

          {/* Actions */}
          <div className="flex justify-end gap-3 pt-4">
            <Button 
              type="button" 
              variant="secondary" 
              onClick={() => onClose(false)}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              loading={isSubmitting}
            >
              {saleId ? "Update Sale" : "Save Sale"}
            </Button>
          </div>
        </form>
      </div>
    </motion.div>
  )
}