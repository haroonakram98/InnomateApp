using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Services;
using InnomateApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class FifoService : IFifoService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<FifoService> _logger;

        public FifoService(IUnitOfWork uow, ILogger<FifoService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<FIFOSaleResultDto> ProcessSaleWithFIFOAsync(
            int productId,
            decimal quantity,
            int saleDetailId,
            string reference,
            string notes)
        {
            var result = new FIFOSaleResultDto
            {
                ProductId = productId,
                QuantitySold = quantity,
                LayersUsed = new List<FIFOLayerDto>()
            };

            try
            {
                // Get available batches (FIFO = oldest first)
                var batches = await _uow.Stock.GetAvailableBatchesForProductAsync(productId);

                if (!batches.Any())
                {
                    result.Success = false;
                    result.Message = "No available stock batches";
                    return result;
                }

                var totalAvailable = batches.Sum(b => b.RemainingQty);
                if (totalAvailable < quantity)
                {
                    result.Success = false;
                    result.Message = $"Insufficient stock. Available: {totalAvailable}, Requested: {quantity}";
                    return result;
                }

                decimal remainingQty = quantity;
                decimal totalCost = 0;

                // Process FIFO - consume oldest batches first
                foreach (var batch in batches.OrderBy(b => b.Purchase!.PurchaseDate))
                {
                    if (remainingQty <= 0) break;

                    decimal qtyToUse = Math.Min(batch.RemainingQty, remainingQty);
                    decimal layerCost = qtyToUse * batch.UnitCost;

                    // Record which batch was used
                    result.LayersUsed.Add(new FIFOLayerDto
                    {
                        PurchaseDetailId = batch.PurchaseDetailId,
                        QuantityUsed = qtyToUse,
                        UnitCost = batch.UnitCost,
                        TotalCost = layerCost
                    });

                    totalCost += layerCost;
                    remainingQty -= qtyToUse;

                    // Update batch remaining quantity
                    batch.RemainingQty -= qtyToUse;
                    await _uow.Stock.UpdatePurchaseDetailAsync(batch);
                }

                // Record stock transaction
                var transaction = new StockTransaction
                {
                    ProductId = productId,
                    TransactionType = 'S', // Sale
                    ReferenceId = saleDetailId,
                    TransactionId = saleDetailId, // Keeping for compatibility
                    Quantity = -quantity, // Negative for outgoing
                    UnitCost = totalCost / quantity,
                    TotalCost = totalCost,
                    CreatedAt = DateTime.Now,
                    Reference = reference,
                    Notes = notes
                };

                await _uow.Stock.AddStockTransactionAsync(transaction);

                // Note: Updating Stock Summary is the caller's responsibility to avoid circular dependencies

                result.Success = true;
                result.TotalCost = totalCost;
                result.AverageCost = totalCost / quantity;
                result.Message = "FIFO processing completed successfully";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing FIFO for product {ProductId}", productId);
                result.Success = false;
                result.Message = $"Error: {ex.Message}";
                return result;
            }
        }
    }
}
