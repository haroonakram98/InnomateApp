export interface StockSummaryDto {
  stockSummaryId: number;
  productId: number;
  productName: string;
  totalIn: number;
  totalOut: number;
  balance: number;
  lastUpdated: string;
  averageCost: number;
  totalValue: number;
}

export interface StockTransactionDto {
  stockTransactionId: number;
  transactionId: number;
  productId: number;
  productName: string;
  transactionType: string;      // "P", "S", "A", "I", "O"
  referenceId: number;
  quantity: number;
  createdAt: string;
  totalCost: number;
  unitCost: number;
  transactionTypeName: string;  // from backend mapping property
}

export interface FifoBatchDto {
  purchaseDetailId: number;
  batchNo: string;
  purchaseDate: string;
  availableQuantity: number;
  unitCost: number;
  expiryDate?: string | null;
}