using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class FifoService : IFifoService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<FifoService> _logger;

        public FifoService(IStockRepository stockRepository, ILogger<FifoService> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<FifoAllocationResult> AllocateStockForSaleAsync(int productId, decimal quantity)
        {
            var result = new FifoAllocationResult();

            try
            {
                // Get available batches in FIFO order
                var availableBatches = await _stockRepository.GetAvailableBatchesForProductAsync(productId);

                if (!availableBatches.Any())
                {
                    result.Success = false;
                    result.ErrorMessage = "No stock available for this product";
                    return result;
                }

                decimal remainingQuantity = quantity;
                decimal totalCost = 0;

                foreach (var batch in availableBatches)
                {
                    if (remainingQuantity <= 0) break;

                    decimal allocatedQuantity = Math.Min(batch.RemainingQty, remainingQuantity);
                    decimal allocationCost = allocatedQuantity * batch.UnitCost;

                    result.Allocations.Add(new FifoAllocationDetail
                    {
                        PurchaseDetailId = batch.PurchaseDetailId,
                        Quantity = allocatedQuantity,
                        UnitCost = batch.UnitCost,
                        TotalCost = allocationCost
                    });

                    totalCost += allocationCost;
                    remainingQuantity -= allocatedQuantity;
                }

                if (remainingQuantity > 0)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Insufficient stock. Only {quantity - remainingQuantity} units available";
                    return result;
                }

                result.Success = true;
                result.TotalCost = totalCost;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error allocating stock for product {ProductId}", productId);
                result.Success = false;
                result.ErrorMessage = "An error occurred during stock allocation";
                return result;
            }
        }

        public async Task<decimal> ReturnStockFromSaleAsync(int saleDetailId, decimal quantity)
        {
            // Implementation logic handled by StockService for now, but keeping this for interface compliance
            // or future direct FIFO reversal if needed.
            // For now, we can throw NotSupportedException or return 0 if it's not strictly used yet,
            // or implement basic logic if required.
            // Based on previous plan, StockService handles this.
            // To satisfy interface, we return 0. (Or we can remove from Interface if unused, but user wants to fix errors).
            return await Task.FromResult(0m);
        }

        public async Task<decimal> CalculateFifoCostAsync(int productId, decimal quantity)
        {
             // Simplified calculation without allocation side effects
             var batches = await _stockRepository.GetAvailableBatchesForProductAsync(productId);
             decimal totalCost = 0;
             decimal remaining = quantity;

             foreach (var batch in batches)
             {
                 if (remaining <= 0) break;
                 decimal take = Math.Min(batch.RemainingQty, remaining);
                 totalCost += take * batch.UnitCost;
                 remaining -= take;
             }
             
             return totalCost;
        }
    }
}
