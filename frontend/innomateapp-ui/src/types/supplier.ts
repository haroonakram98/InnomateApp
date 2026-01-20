// src/types/supplier.ts
export interface SupplierDTO {
  supplierId: number;
  name: string;
  email: string;
  phone: string;
  address?: string;
  contactPerson?: string;
  notes?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateSupplierDTO {
  name: string;
  email: string;
  phone: string;
  address?: string;
  contactPerson?: string;
  notes?: string;
}

export interface UpdateSupplierDTO {
  supplierId: number;
  name: string;
  email: string;
  phone: string;
  address?: string;
  contactPerson?: string;
  notes?: string;
}

export interface SupplierWithStatsDTO extends SupplierDTO {
  totalPurchases: number;
  totalPurchaseAmount: number;
  pendingPurchases: number;
  lastPurchaseDate?: string;
}

export interface SupplierDetailDto extends SupplierDTO {
  totalPurchases: number;
  totalPurchaseAmount: number;
  pendingPurchases: number;
  lastPurchaseDate?: string;
  recentPurchases?: PurchaseSummaryDto[];
}

export interface PurchaseSummaryDto {
  purchaseId: number;
  purchaseNumber: string;
  purchaseDate: string;
  status: string;
  totalAmount: number;
}

export interface SupplierStats {
  totalPurchases: number;
  totalPurchaseAmount: number;
  pendingPurchases: number;
  lastPurchaseDate?: string;
  averagePurchaseAmount: number;
  completedPurchases: number;
}

export interface SupplierLookupDTO {
  supplierId: number;
  name: string;
  contactPerson?: string;
}