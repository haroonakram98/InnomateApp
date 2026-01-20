// src/types/sale.d.ts
export type SaleDetailDTO = {
  saleDetailId?: number;
  productId: number;
  productName?: string;
  quantity: number;
  unitPrice: number;
  total?: number;
  purchaseDetailId?: number | null;
  costPrice?: number;
  profitMargin?: number;
  discount?: number;
  discountPercentage?: number;
  netTotal?: number;
  discountType?: string;
};

export type PaymentDTO = {
  paymentId?: number;
  paymentMethod: "Cash" | "Card" | "Bank Transfer" | "Mobile Payment";
  amount: number;
  referenceNo?: string | null;
  paymentDate?: string;
};

export type SaleDTO = {
  saleId: number;
  saleDate: string;
  invoiceNo?: string;
  totalAmount: number;
  createdBy: number;
  createdAt: string;
  customerId: number;
  balanceAmount: number;
  isFullyPaid: boolean;
  paidAmount: number;

  // New fields from database
  deletedAt?: string | null;
  isDeleted?: boolean;
  profitMargin?: number;
  totalCost?: number;
  totalProfit?: number;
  updatedAt?: string;
  discount?: number;
  discountPercentage?: number;
  subTotal?: number;
  discountType?: string;

  // Related data
  saleDetails: SaleDetailDTO[];
  payments: PaymentDTO[];
  customer?: CustomerDTO;


  // Optional fields for UI
  saleType?: string;
  dueDate?: string;
  isEstimate?: boolean;
};

export type CreateSaleDetailDTO = {
  productId: number;
  quantity: number;
  unitPrice: number;
  discount?: number;
  discountPercentage?: number;
  discountType?: string;
};

export type CreatePaymentDTO = {
  paymentMethod: "Cash" | "Card" | "Bank Transfer" | "Mobile Payment";
  amount: number;
  referenceNo?: string | null;
};

export type CreateSaleDTO = {
  customerId: number;
  invoiceNo?: string;
  createdBy: number;
  paidAmount: number;
  balanceAmount: number;
  isFullyPaid: boolean;
  discount?: number;
  discountPercentage?: number;
  discountType?: string;
  saleDetails: CreateSaleDetailDTO[];
  payments: CreatePaymentDTO[];
};

// Additional types for API responses
export type SalesSummary = {
  totalSales: number;
  totalRevenue: number;
  totalProfit: number;
  averageTransaction: number;
  totalItemsSold: number;
};

export type SalesTrend = {
  period: string;
  sales: number;
  revenue: number;
  profit: number;
};

export type TopProduct = {
  productId: number;
  productName: string;
  quantitySold: number;
  totalRevenue: number;
  profit: number;
};