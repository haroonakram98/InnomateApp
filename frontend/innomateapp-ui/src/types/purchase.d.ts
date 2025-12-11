// src/types/purchase.ts
export interface PurchaseDetailDTO {
  purchaseDetailId?: number;
  productId: number;
  productName?: string;
  productCode?: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
  remainingQty?: number;
  expiryDate?: string;
  batchNo?: string;
}

export interface PurchaseDTO {
  purchaseId: number;
  purchaseNumber: string;
  supplierId: number;
  supplierName?: string;
  purchaseDate: string;
  receivedDate?: string;
  status: 'Pending' | 'Received' | 'Cancelled';
  notes?: string;
  totalAmount: number;
  createdAt: string;
  purchaseDetails: PurchaseDetailDTO[];
}

export interface CreatePurchaseDTO {
  supplierId: number;
  purchaseDate: string;
  notes?: string;
  purchaseDetails: CreatePurchaseDetailDTO[];
  invoiceNo: string; // add this
  createdBy: number; // add this
}

export interface CreatePurchaseDetailDTO {
  productId: number;
  quantity: number;
  unitCost: number;
  expiryDate?: string;
  batchNo?: string;
}

export interface UpdatePurchaseDTO {
  notes?: string;
  status?: string;
}