import { create } from "zustand";
import {
    StockSummaryDto,
    StockTransactionDto,
    FifoBatchDto,
    StockMovementDto,
    FIFOSaleRequestDto
} from "@/types/stock.js";
import { stocksApi } from "@/api/stocks.js";
import { useToastStore } from "@/store/useToastStore.js";

interface StockState {
    summaries: StockSummaryDto[];
    selectedProductTransactions: StockTransactionDto[];
    selectedProductBatches: FifoBatchDto[];
    isLoading: boolean;
    error: string | null;

    actions: {
        fetchSummaries: (search?: string) => Promise<void>;
        fetchTransactions: (productId: number) => Promise<void>;
        fetchBatches: (productId: number) => Promise<void>;
        refreshSummary: (productId: number) => Promise<void>;
        recordMovement: (movement: StockMovementDto) => Promise<void>;
        processFIFOSale: (request: FIFOSaleRequestDto) => Promise<void>;
        clearError: () => void;
    };
}

export const useStockStore = create<StockState>((set, get) => ({
    summaries: [],
    selectedProductTransactions: [],
    selectedProductBatches: [],
    isLoading: false,
    error: null,

    actions: {
        fetchSummaries: async (search?: string) => {
            set({ isLoading: true, error: null });
            try {
                const summaries = await stocksApi.getAllSummaries(search);
                set({ summaries, isLoading: false });
            } catch (error: any) {
                const message = error.response?.data || "Failed to fetch stock summaries";
                set({ error: message, isLoading: false });
            }
        },

        fetchTransactions: async (productId: number) => {
            set({ isLoading: true, error: null });
            try {
                const transactions = await stocksApi.getTransactions(productId);
                set({ selectedProductTransactions: transactions, isLoading: false });
            } catch (error: any) {
                const message = error.response?.data || "Failed to fetch stock transactions";
                set({ error: message, isLoading: false });
            }
        },

        fetchBatches: async (productId: number) => {
            set({ isLoading: true, error: null });
            try {
                const batches = await stocksApi.getBatches(productId);
                set({ selectedProductBatches: batches, isLoading: false });
            } catch (error: any) {
                const message = error.response?.data || "Failed to fetch FIFO batches";
                set({ error: message, isLoading: false });
            }
        },

        refreshSummary: async (productId: number) => {
            const toast = useToastStore.getState().push;
            try {
                await stocksApi.refreshSummary(productId);
                const updatedSummary = await stocksApi.getSummaryByProductId(productId);
                set((state) => ({
                    summaries: state.summaries.map((s) =>
                        s.productId === productId ? updatedSummary : s
                    ),
                }));
                toast("Stock summary refreshed successfully", "success");
            } catch (error: any) {
                const message = error.response?.data || "Failed to refresh summary";
                toast(message, "error");
            }
        },

        recordMovement: async (movement: StockMovementDto) => {
            const toast = useToastStore.getState().push;
            try {
                await stocksApi.recordMovement(movement);
                await get().actions.fetchSummaries();
                toast("Stock movement recorded successfully", "success");
            } catch (error: any) {
                const message = error.response?.data || "Failed to record movement";
                toast(message, "error");
                throw error;
            }
        },

        processFIFOSale: async (request: FIFOSaleRequestDto) => {
            const toast = useToastStore.getState().push;
            try {
                const result = await stocksApi.processFIFOSale(request);
                if (result.success) {
                    await get().actions.fetchSummaries();
                    toast(result.message, "success");
                } else {
                    toast(result.message, "error");
                }
            } catch (error: any) {
                const message = error.response?.data || "Failed to process FIFO sale";
                toast(message, "error");
                throw error;
            }
        },

        clearError: () => set({ error: null }),
    },
}));

// Selector hooks
export const useStockSummaries = () => useStockStore((state) => state.summaries);
export const useStockTransactions = () => useStockStore((state) => state.selectedProductTransactions);
export const useStockBatches = () => useStockStore((state) => state.selectedProductBatches);
export const useStocksLoading = () => useStockStore((state) => state.isLoading);
export const useStocksError = () => useStockStore((state) => state.error);
export const useStockActions = () => useStockStore((state) => state.actions);
