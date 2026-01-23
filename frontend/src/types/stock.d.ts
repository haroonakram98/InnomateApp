export interface StockSummaryDto {
  stockSummaryId: number;
  productId: number;
  productName: string;
  totalIn: number;
  totalOut: number;
  balance: number;
  averageCost: number;
  totalValue: number;
  lastUpdated: string;
}

export interface StockTransactionDto {
  stockTransactionId: number;
  transactionId: number;
  productId: number;
  productName: string;
  transactionType: string; // "P", "S", "A"
  referenceId: number;
  quantity: number;
  unitCost: number;
  totalCost: number;
  createdAt: string;
  transactionTypeName: string;
}

export interface FifoBatchDto {
  purchaseDetailId: number;
  batchNo: string;
  purchaseDate: string;
  availableQuantity: number;
  unitCost: number;
  expiryDate?: string | null;
}

export interface StockMovementDto {
  productId: number;
  transactionType: string;
  referenceId: number;
  quantity: number;
  unitCost: number;
  transactionDate?: string;
  notes?: string;
}

export interface FIFOSaleRequestDto {
  productId: number;
  quantity: number;
  saleReferenceId: number;
  notes?: string;
}

export interface FIFOSaleResultDto {
  productId: number;
  quantitySold: number;
  totalCost: number;
  averageCost: number;
  layersUsed: FIFOLayerDto[];
  success: boolean;
  message: string;
}

export interface FIFOLayerDto {
  purchaseDetailId: number;
  quantityUsed: number;
  unitCost: number;
  totalCost: number;
}