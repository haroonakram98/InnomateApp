// src/types/sale.d.ts
export type SaleDetailDTO = {
  productId: number
  quantity: number
  unitPrice: number
  total?: number
  purchaseDetailId?: number | null
}

export type PaymentDTO = {
  paymentMethod: "Cash" | "Card" | "Bank Transfer" | "Mobile Payment"
  amount: number
  referenceNo?: string | null
}

export type SaleDTO = {
  saleId?: number
  saleDate?: string
  customerId: number
  invoiceNo?: string
  totalAmount: number
  createdBy?: number
  createdAt?: string
  saleDetails: SaleDetailDTO[]
  payments: PaymentDTO[]
  customerName?: string
}

// Additional updates can be made here if needed
