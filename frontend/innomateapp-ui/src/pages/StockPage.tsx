// components/stocks/StockPage.tsx
import { useEffect, useMemo, useState } from "react";
import {
  Search,
  RefreshCw,
  List,
  Layers,
  ChevronLeft,
  ChevronRight,
  X,
} from "lucide-react";
import MainLayout from "@/components/layout/MainLayout.js";
import { useTheme } from "@/hooks/useTheme.js";
import { StocksApi } from "@/api/stocks.js";
import {
  StockSummaryDto,
  StockTransactionDto,
  FifoBatchDto,
} from "@/types/stock.js";

export default function StockPage() {
  const { isDark } = useTheme();

  const [summaries, setSummaries] = useState<StockSummaryDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [search, setSearch] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  // Modals
  const [selectedProduct, setSelectedProduct] = useState<StockSummaryDto | null>(null);
  const [transactions, setTransactions] = useState<StockTransactionDto[]>([]);
  const [batches, setBatches] = useState<FifoBatchDto[]>([]);
  const [isTransactionsModalOpen, setIsTransactionsModalOpen] = useState(false);
  const [isBatchesModalOpen, setIsBatchesModalOpen] = useState(false);
  const [modalLoading, setModalLoading] = useState(false);
  const [modalError, setModalError] = useState<string | null>(null);

  const theme = {
    bg: isDark ? "bg-gray-900" : "bg-gray-50",
    bgCard: isDark ? "bg-gray-800" : "bg-white",
    bgSecondary: isDark ? "bg-gray-700" : "bg-gray-50",
    bgHover: isDark ? "hover:bg-gray-700" : "hover:bg-gray-100",
    text: isDark ? "text-gray-100" : "text-gray-900",
    textSecondary: isDark ? "text-gray-400" : "text-gray-600",
    textMuted: isDark ? "text-gray-500" : "text-gray-500",
    border: isDark ? "border-gray-700" : "border-gray-200",
    borderLight: isDark ? "border-gray-600" : "border-gray-100",
    input: isDark
      ? "bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-400 focus:border-blue-500"
      : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-blue-500",
    buttonPrimary: "bg-blue-600 hover:bg-blue-700 text-white",
    buttonSecondary: isDark
      ? "bg-gray-700 hover:bg-gray-600 text-gray-100"
      : "bg-gray-200 hover:bg-gray-300 text-gray-700",
    chipPositive: isDark
      ? "bg-green-900/40 text-green-300 border border-green-700"
      : "bg-green-50 text-green-700 border border-green-200",
    chipNegative: isDark
      ? "bg-red-900/40 text-red-300 border border-red-700"
      : "bg-red-50 text-red-700 border border-red-200",
  };

  // Load all stock summaries
  const loadSummaries = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await StocksApi.getAllSummaries();
      setSummaries(data);
    } catch (err: any) {
      setError(err?.response?.data || "Failed to load stock summaries");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSummaries();
  }, []);

  // Filter + paginate
  const filteredSummaries = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return summaries;
    return summaries.filter(
      (s) =>
        s.productName.toLowerCase().includes(q) ||
        s.productId.toString().includes(q)
    );
  }, [summaries, search]);

  useEffect(() => {
    setCurrentPage(1);
  }, [search]);

  const totalPages = Math.max(
    1,
    Math.ceil(filteredSummaries.length / itemsPerPage)
  );
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentSummaries = filteredSummaries.slice(
    startIndex,
    startIndex + itemsPerPage
  );

  const isEmpty = filteredSummaries.length === 0;
  const isSearching = search.length > 0;

  const clearError = () => setError(null);
  const clearModalError = () => setModalError(null);

  // Format helpers
  const formatNumber = (value: number, digits = 2) =>
    value?.toLocaleString(undefined, {
      minimumFractionDigits: digits,
      maximumFractionDigits: digits,
    });

  const formatDate = (value: string | null | undefined) => {
    if (!value) return "-";
    const d = new Date(value);
    if (Number.isNaN(d.getTime())) return value;
    return d.toLocaleString();
  };

  const formatTransactionType = (tx: StockTransactionDto) => {
    // Backend already sets TransactionTypeName; use that as main label.
    return tx.transactionTypeName || tx.transactionType || "Unknown";
  };

  // Actions
  const handleRefreshSummary = async (summary: StockSummaryDto) => {
    try {
      setModalError(null);
      await StocksApi.refreshSummary(summary.productId);
      await loadSummaries();
    } catch (err: any) {
      setModalError(err?.response?.data || "Failed to refresh stock summary");
    }
  };

  const openTransactionsModal = async (summary: StockSummaryDto) => {
    setSelectedProduct(summary);
    setModalLoading(true);
    setModalError(null);
    setIsTransactionsModalOpen(true);
    try {
      const data = await StocksApi.getTransactions(summary.productId);
      setTransactions(data);
    } catch (err: any) {
      setModalError(
        err?.response?.data || "Failed to load stock transactions"
      );
    } finally {
      setModalLoading(false);
    }
  };

  const openBatchesModal = async (summary: StockSummaryDto) => {
    setSelectedProduct(summary);
    setModalLoading(true);
    setModalError(null);
    setIsBatchesModalOpen(true);
    try {
      const data = await StocksApi.getBatches(summary.productId);
      setBatches(data);
    } catch (err: any) {
      setModalError(err?.response?.data || "Failed to load FIFO batches");
    } finally {
      setModalLoading(false);
    }
  };

  const closeTransactionsModal = () => {
    setIsTransactionsModalOpen(false);
    setTransactions([]);
    setSelectedProduct(null);
    setModalError(null);
  };

  const closeBatchesModal = () => {
    setIsBatchesModalOpen(false);
    setBatches([]);
    setSelectedProduct(null);
    setModalError(null);
  };

  return (
    <MainLayout>
      <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
        {/* Header */}
        <div className="mb-6 flex flex-col gap-1">
          <h2 className={`text-xl font-bold ${theme.text}`}>
            Inventory & Stock
          </h2>
          <p className={`text-sm ${theme.textSecondary}`}>
            View live stock balances, movements, and FIFO batches for each
            product.
          </p>
        </div>

        {/* Error Alert */}
        {error && (
          <div
            className={`mb-4 p-4 rounded-lg border ${
              isDark
                ? "bg-red-900/20 border-red-800 text-red-200"
                : "bg-red-50 border-red-200 text-red-700"
            }`}
          >
            <div className="flex justify-between items-start gap-4">
              <span>{error}</span>
              <button
                onClick={clearError}
                className={`p-1 rounded ${
                  isDark ? "hover:bg-red-800" : "hover:bg-red-100"
                }`}
              >
                <X size={16} />
              </button>
            </div>
          </div>
        )}

        {/* Search + Refresh All */}
        <div className="flex justify-between items-center mb-6 gap-4">
          <div className="relative w-80">
            <Search
              className={`absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 ${theme.textMuted}`}
            />
            <input
              type="text"
              placeholder="Search by product or ID..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className={`w-full pl-10 pr-8 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 ${theme.input}`}
            />
            {search && (
              <button
                onClick={() => setSearch("")}
                className={`absolute right-2 top-1/2 transform -translate-y-1/2 p-1 rounded ${
                  isDark ? "hover:bg-gray-600" : "hover:bg-gray-200"
                }`}
              >
                <X size={14} className={theme.textMuted} />
              </button>
            )}
          </div>

          <button
            onClick={loadSummaries}
            className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonSecondary}`}
          >
            <RefreshCw className="w-4 h-4" />
            Refresh All
          </button>
        </div>

        {/* Table */}
        <div
          className={`rounded-lg border shadow-sm flex-1 flex flex-col ${theme.bgCard} ${theme.border}`}
        >
          {/* Header row */}
          <div
            className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-xs font-semibold uppercase tracking-wide ${theme.bgSecondary} ${theme.border} ${theme.textSecondary}`}
          >
            <div className="col-span-2">Product</div>
            <div className="col-span-1 text-right">In</div>
            <div className="col-span-1 text-right">Out</div>
            <div className="col-span-1 text-right">Balance</div>
            <div className="col-span-1 text-right">Avg. Cost</div>
            <div className="col-span-2 text-right">Total Value</div>
            <div className="col-span-2">Last Updated</div>
            <div className="col-span-2 text-center">Actions</div>
          </div>

          {/* Body */}
          <div className="flex-1">
            {loading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
              </div>
            ) : isEmpty ? (
              <div
                className={`flex flex-col items-center justify-center h-32 ${theme.textMuted}`}
              >
                {isSearching ? (
                  <>
                    <p>No stock records found for “{search}”.</p>
                    <button
                      onClick={() => setSearch("")}
                      className="mt-2 text-blue-500 hover:text-blue-700 text-sm"
                    >
                      Clear search
                    </button>
                  </>
                ) : (
                  <p>No stock data available.</p>
                )}
              </div>
            ) : (
              currentSummaries.map((s, index) => {
                const isNegative = s.balance < 0;
                return (
                  <div
                    key={s.stockSummaryId}
                    className={`grid grid-cols-12 gap-4 px-6 py-3 border-b text-sm items-center transition-colors ${
                      index % 2 === 0
                        ? isDark
                          ? "bg-gray-800"
                          : "bg-white"
                        : isDark
                        ? "bg-gray-900/40"
                        : "bg-gray-50"
                    } ${theme.borderLight} ${theme.bgHover}`}
                  >
                    {/* Product */}
                    <div className="col-span-2 flex flex-col">
                      <span className={`font-medium ${theme.text}`}>
                        {s.productName || "Unnamed Product"}
                      </span>
                      <span className={`text-xs ${theme.textMuted}`}>
                        ID: {s.productId}
                      </span>
                    </div>

                    {/* Total In */}
                    <div className="col-span-1 text-right">
                      <span className={theme.textSecondary}>
                        {formatNumber(s.totalIn)}
                      </span>
                    </div>

                    {/* Total Out */}
                    <div className="col-span-1 text-right">
                      <span className={theme.textSecondary}>
                        {formatNumber(s.totalOut)}
                      </span>
                    </div>

                    {/* Balance */}
                    <div className="col-span-1 text-right">
                      <span
                        className={`inline-flex items-center justify-end px-2 py-0.5 rounded-full text-xs font-medium ${
                          isNegative ? theme.chipNegative : theme.chipPositive
                        }`}
                      >
                        {formatNumber(s.balance)}
                      </span>
                    </div>

                    {/* Average Cost */}
                    <div className="col-span-1 text-right">
                      <span className={theme.textSecondary}>
                        {formatNumber(s.averageCost)}
                      </span>
                    </div>

                    {/* Total Value */}
                    <div className="col-span-2 text-right">
                      <span className={theme.text}>
                        {formatNumber(s.totalValue)}
                      </span>
                    </div>

                    {/* Last Updated */}
                    <div className="col-span-2">
                      <span className={`text-xs ${theme.textSecondary}`}>
                        {formatDate(s.lastUpdated)}
                      </span>
                    </div>

                    {/* Actions */}
                    <div className="col-span-2 flex justify-center gap-2">
                      <button
                        onClick={() => openTransactionsModal(s)}
                        className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${theme.buttonSecondary}`}
                        title="View transactions"
                      >
                        <List className="w-4 h-4" />
                        Movements
                      </button>
                      <button
                        onClick={() => openBatchesModal(s)}
                        className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${theme.buttonSecondary}`}
                        title="View FIFO batches"
                      >
                        <Layers className="w-4 h-4" />
                        Batches
                      </button>
                      <button
                        onClick={() => handleRefreshSummary(s)}
                        className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${theme.buttonPrimary}`}
                        title="Recalculate summary"
                      >
                        <RefreshCw className="w-4 h-4" />
                        Refresh
                      </button>
                    </div>
                  </div>
                );
              })
            )}
          </div>

          {/* Pagination */}
          {!isEmpty && !loading && (
            <div
              className={`px-6 py-4 border-t flex justify-between items-center ${theme.border} ${theme.bgCard}`}
            >
              <div className={`text-sm ${theme.textSecondary}`}>
                Showing {startIndex + 1} to{" "}
                {Math.min(startIndex + itemsPerPage, filteredSummaries.length)}{" "}
                of {filteredSummaries.length} records
              </div>
              <div className="flex items-center gap-2">
                <button
                  onClick={() =>
                    setCurrentPage((p) => Math.max(1, p - 1))
                  }
                  disabled={currentPage === 1}
                  className={`p-2 rounded border transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${
                    isDark
                      ? "border-gray-600 hover:bg-gray-700"
                      : "border-gray-300 hover:bg-gray-100"
                  }`}
                >
                  <ChevronLeft size={16} className={theme.text} />
                </button>
                <span className={`text-sm px-3 ${theme.text}`}>
                  Page {currentPage} of {totalPages}
                </span>
                <button
                  onClick={() =>
                    setCurrentPage((p) => Math.min(totalPages, p + 1))
                  }
                  disabled={currentPage === totalPages}
                  className={`p-2 rounded border transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${
                    isDark
                      ? "border-gray-600 hover:bg-gray-700"
                      : "border-gray-300 hover:bg-gray-100"
                  }`}
                >
                  <ChevronRight size={16} className={theme.text} />
                </button>
              </div>
            </div>
          )}
        </div>

        {/* Modal shared error */}
        {modalError && (
          <div
            className={`mt-4 p-3 rounded-lg border text-sm max-w-xl ${
              isDark
                ? "bg-red-900/20 border-red-800 text-red-200"
                : "bg-red-50 border-red-200 text-red-700"
            }`}
          >
            <div className="flex justify-between items-start gap-4">
              <span>{modalError}</span>
              <button
                onClick={clearModalError}
                className={`p-1 rounded ${
                  isDark ? "hover:bg-red-800" : "hover:bg-red-100"
                }`}
              >
                <X size={14} />
              </button>
            </div>
          </div>
        )}

        {/* Transactions Modal */}
        {isTransactionsModalOpen && selectedProduct && (
          <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4">
            <div
              className={`w-full max-w-4xl rounded-xl shadow-2xl overflow-hidden ${theme.bgCard}`}
            >
              <div
                className={`flex items-center justify-between px-6 py-4 border-b ${theme.border}`}
              >
                <div>
                  <h3 className={`text-lg font-semibold ${theme.text}`}>
                    Stock Movements – {selectedProduct.productName}
                  </h3>
                  <p className={`text-xs ${theme.textSecondary}`}>
                    Product ID: {selectedProduct.productId}
                  </p>
                </div>
                <button
                  onClick={closeTransactionsModal}
                  className={`p-1.5 rounded-lg ${
                    isDark ? "hover:bg-gray-700" : "hover:bg-gray-100"
                  }`}
                >
                  <X size={18} />
                </button>
              </div>

              <div className="p-6 max-h-[70vh] overflow-auto">
                {modalLoading ? (
                  <div className="flex items-center justify-center h-32">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                  </div>
                ) : transactions.length === 0 ? (
                  <p className={`text-sm ${theme.textSecondary}`}>
                    No stock transactions found for this product.
                  </p>
                ) : (
                  <div className="space-y-2 text-sm">
                    <div
                      className={`grid grid-cols-12 gap-4 font-semibold text-xs uppercase border-b pb-2 ${theme.textSecondary}`}
                    >
                      <div className="col-span-2">Date</div>
                      <div className="col-span-2">Type</div>
                      <div className="col-span-2 text-right">Quantity</div>
                      <div className="col-span-2 text-right">Unit Cost</div>
                      <div className="col-span-2 text-right">Total Cost</div>
                      <div className="col-span-2 text-right">Ref ID</div>
                    </div>

                    {transactions.map((tx, i) => (
                      <div
                        key={tx.stockTransactionId}
                        className={`grid grid-cols-12 gap-4 py-2 border-b last:border-0 ${
                          theme.borderLight
                        } ${
                          i % 2 === 0
                            ? isDark
                              ? "bg-gray-800/40"
                              : "bg-gray-50"
                            : ""
                        }`}
                      >
                        <div className="col-span-2">
                          <span className={theme.textSecondary}>
                            {formatDate(tx.createdAt)}
                          </span>
                        </div>
                        <div className="col-span-2">
                          <span className={theme.text}>
                            {formatTransactionType(tx)}
                          </span>
                        </div>
                        <div className="col-span-2 text-right">
                          <span className={theme.textSecondary}>
                            {formatNumber(tx.quantity)}
                          </span>
                        </div>
                        <div className="col-span-2 text-right">
                          <span className={theme.textSecondary}>
                            {formatNumber(tx.unitCost)}
                          </span>
                        </div>
                        <div className="col-span-2 text-right">
                          <span className={theme.text}>
                            {formatNumber(tx.totalCost)}
                          </span>
                        </div>
                        <div className="col-span-2 text-right">
                          <span className={theme.textSecondary}>
                            {tx.referenceId}
                          </span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div
                className={`px-6 py-3 border-t flex justify-end ${theme.border}`}
              >
                <button
                  onClick={closeTransactionsModal}
                  className={`px-4 py-2 rounded-lg text-sm ${theme.buttonSecondary}`}
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Batches Modal */}
        {isBatchesModalOpen && selectedProduct && (
          <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4">
            <div
              className={`w-full max-w-4xl rounded-xl shadow-2xl overflow-hidden ${theme.bgCard}`}
            >
              <div
                className={`flex items-center justify-between px-6 py-4 border-b ${theme.border}`}
              >
                <div>
                  <h3 className={`text-lg font-semibold ${theme.text}`}>
                    FIFO Batches – {selectedProduct.productName}
                  </h3>
                  <p className={`text-xs ${theme.textSecondary}`}>
                    Product ID: {selectedProduct.productId}
                  </p>
                </div>
                <button
                  onClick={closeBatchesModal}
                  className={`p-1.5 rounded-lg ${
                    isDark ? "hover:bg-gray-700" : "hover:bg-gray-100"
                  }`}
                >
                  <X size={18} />
                </button>
              </div>

              <div className="p-6 max-h-[70vh] overflow-auto">
                {modalLoading ? (
                  <div className="flex items-center justify-center h-32">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                  </div>
                ) : batches.length === 0 ? (
                  <p className={`text-sm ${theme.textSecondary}`}>
                    No available FIFO batches for this product.
                  </p>
                ) : (
                  <div className="space-y-2 text-sm">
                    <div
                      className={`grid grid-cols-12 gap-4 font-semibold text-xs uppercase border-b pb-2 ${theme.textSecondary}`}
                    >
                      <div className="col-span-3">Batch</div>
                      <div className="col-span-2">Purchase Date</div>
                      <div className="col-span-2 text-right">Available</div>
                      <div className="col-span-2 text-right">Unit Cost</div>
                      <div className="col-span-3">Expiry</div>
                    </div>

                    {batches.map((b, i) => (
                      <div
                        key={b.purchaseDetailId}
                        className={`grid grid-cols-12 gap-4 py-2 border-b last:border-0 ${
                          theme.borderLight
                        } ${
                          i % 2 === 0
                            ? isDark
                              ? "bg-gray-800/40"
                              : "bg-gray-50"
                            : ""
                        }`}
                      >
                        <div className="col-span-3">
                          <span className={theme.text}>
                            {b.batchNo || `Batch #${b.purchaseDetailId}`}
                          </span>
                        </div>
                        <div className="col-span-2">
                          <span className={theme.textSecondary}>
                            {formatDate(b.purchaseDate)}
                          </span>
                        </div>
                        <div className="col-span-2 text-right">
                          <span className={theme.textSecondary}>
                            {formatNumber(b.availableQuantity)}
                          </span>
                        </div>
                        <div className="col-span-2 text-right">
                          <span className={theme.textSecondary}>
                            {formatNumber(b.unitCost)}
                          </span>
                        </div>
                        <div className="col-span-3">
                          <span className={theme.textSecondary}>
                            {b.expiryDate ? formatDate(b.expiryDate) : "—"}
                          </span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div
                className={`px-6 py-3 border-t flex justify-end ${theme.border}`}
              >
                <button
                  onClick={closeBatchesModal}
                  className={`px-4 py-2 rounded-lg text-sm ${theme.buttonSecondary}`}
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </MainLayout>
  );
}
