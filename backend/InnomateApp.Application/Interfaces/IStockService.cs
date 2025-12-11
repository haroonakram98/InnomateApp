// IStockService.cs
using InnomateApp.Application.DTOs;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces
{
    public interface IStockService
    {
        // Stock Summary
        Task<StockSummaryDto?> GetStockSummaryByProductIdAsync(int productId);
        Task<IEnumerable<StockSummaryDto>> GetAllStockSummariesAsync();

        // Stock Transactions
        Task<IEnumerable<StockTransactionDto>> GetStockTransactionsByProductAsync(int productId);

        // Stock Movements
        Task<bool> RecordStockMovementAsync(StockMovementDto movement);
        Task<FIFOSaleResultDto> ProcessFIFOSaleAsync(int productId, decimal quantity, int saleReferenceId);

        // Stock Information
        Task<decimal> GetProductStockBalanceAsync(int productId);
        Task<decimal> GetProductStockValueAsync(int productId);

        // Batch Operations
        Task<bool> UpdateStockSummaryAsync(int productId);
        Task<IEnumerable<FifoBatchDto>> GetAvailableBatchesAsync(int productId);
        Task<FIFOSaleResultDto> ProcessSaleWithFIFOAsync(
            int productId,
            decimal quantity,
            int saleDetailId);

        // ✅ NEW: Validate stock before sale
        Task<StockValidationResult> ValidateStockAvailabilityAsync(
            List<(int ProductId, decimal Quantity)> items);

        // ✅ NEW: Restore stock (for returns/cancellations)
        Task RestoreStockFromSaleDetailAsync(SaleDetail saleDetail);

    }
}