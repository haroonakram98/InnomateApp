// api/stocks.ts
import { StockSummaryDto, StockTransactionDto, FifoBatchDto } from "@/types/stock.js";
import axios from "@/lib/utils/axios.js"; // adjust path to your axios instance

export const StocksApi = {
  getAllSummaries: async (): Promise<StockSummaryDto[]> => {
    const res = await axios.get<StockSummaryDto[]>("/stocks/summary");
    return res.data;
  },

  getTransactions: async (productId: number): Promise<StockTransactionDto[]> => {
    const res = await axios.get<StockTransactionDto[]>(`/stocks/transactions/${productId}`);
    return res.data;
  },

  getBatches: async (productId: number): Promise<FifoBatchDto[]> => {
    const res = await axios.get<FifoBatchDto[]>(`/stocks/batches/${productId}`);
    return res.data;
  },

  refreshSummary: async (productId: number): Promise<void> => {
    await axios.put(`/stocks/summary/${productId}/refresh`);
  },
};
