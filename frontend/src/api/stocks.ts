import {
  StockSummaryDto,
  StockTransactionDto,
  FifoBatchDto,
  StockMovementDto,
  FIFOSaleRequestDto,
  FIFOSaleResultDto
} from "@/types/stock.js";
import axios from "@/lib/utils/axios.js";

export const stocksApi = {
  getAllSummaries: async (search?: string): Promise<StockSummaryDto[]> => {
    const res = await axios.get<StockSummaryDto[]>("/stocks/summary", {
      params: { search }
    });
    return res.data;
  },

  getSummaryByProductId: async (productId: number): Promise<StockSummaryDto> => {
    const res = await axios.get<StockSummaryDto>(`/stocks/summary/${productId}`);
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

  recordMovement: async (movement: StockMovementDto): Promise<void> => {
    await axios.post("/stocks/movement", movement);
  },

  processFIFOSale: async (request: FIFOSaleRequestDto): Promise<FIFOSaleResultDto> => {
    const res = await axios.post<FIFOSaleResultDto>("/stocks/sale/fifo", request);
    return res.data;
  },

  getBalance: async (productId: number): Promise<number> => {
    const res = await axios.get<number>(`/stocks/balance/${productId}`);
    return res.data;
  },

  getValue: async (productId: number): Promise<number> => {
    const res = await axios.get<number>(`/stocks/value/${productId}`);
    return res.data;
  }
};
