// StockService.cs
using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IStockSummaryRepository _stockSummaryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StockService> _logger;

        public StockService(
            IStockRepository stockRepository,
            IStockSummaryRepository stockSummaryRepository,
            IProductRepository productRepository,
            ISaleRepository saleRepository,
            IMapper mapper,
            ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _stockSummaryRepository = stockSummaryRepository;
            _productRepository = productRepository;
            _saleRepository = saleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StockSummaryDto?> GetStockSummaryByProductIdAsync(int productId)
        {
            try
            {
                var stockSummary = await _stockRepository.GetStockSummaryByProductIdAsync(productId);
                if (stockSummary == null) return null;

                var dto = _mapper.Map<StockSummaryDto>(stockSummary);
                dto.ProductName = stockSummary.Product?.Name ?? string.Empty;
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock summary for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<StockSummaryDto>> GetAllStockSummariesAsync()
        {
            try
            {
                var stockSummaries = await _stockSummaryRepository.GetAllAsync();
                return stockSummaries.Select(ss => new StockSummaryDto
                {
                    StockSummaryId = ss.StockSummaryId,
                    ProductId = ss.ProductId,
                    ProductName = ss.Product?.Name ?? string.Empty,
                    TotalIn = ss.TotalIn,
                    TotalOut = ss.TotalOut,
                    Balance = ss.Balance,
                    AverageCost = ss.AverageCost,
                    TotalValue = ss.TotalValue,
                    LastUpdated = ss.LastUpdated
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stock summaries");
                throw;
            }
        }

        public async Task<IEnumerable<StockTransactionDto>> GetStockTransactionsByProductAsync(int productId)
        {
            try
            {
                var transactions = await _stockRepository.GetStockTransactionsByProductAsync(productId);
                return transactions.Select(t => new StockTransactionDto
                {
                    StockTransactionId = t.StockTransactionId,
                    TransactionId = t.TransactionId,
                    ProductId = t.ProductId,
                    ProductName = t.Product?.Name ?? string.Empty,
                    TransactionType = t.TransactionType.ToString(),
                    ReferenceId = t.ReferenceId,
                    Quantity = t.Quantity,
                    UnitCost = t.UnitCost,
                    TotalCost = t.TotalCost,
                    CreatedAt = t.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock transactions for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<FifoBatchDto>> GetAvailableBatchesAsync(int productId)
        {
            try
            {
                var batches = await _stockRepository.GetAvailableBatchesForProductAsync(productId);
                return batches.Select(b => new FifoBatchDto
                {
                    PurchaseDetailId = b.PurchaseDetailId,
                    BatchNo = b.BatchNo ?? $"BATCH-{b.PurchaseDetailId}",
                    PurchaseDate = b.Purchase?.PurchaseDate ?? DateTime.Now,
                    AvailableQuantity = b.RemainingQty,
                    UnitCost = b.UnitCost,
                    ExpiryDate = b.ExpiryDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available batches for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> RecordStockMovementAsync(StockMovementDto movement)
        {
            try
            {
                var transaction = new StockTransaction
                {
                    TransactionId = movement.ReferenceId,
                    ProductId = movement.ProductId,
                    TransactionType = movement.TransactionType[0], // Convert string to char
                    ReferenceId = movement.ReferenceId,
                    Quantity = movement.Quantity,
                    UnitCost = movement.UnitCost,
                    TotalCost = movement.Quantity * movement.UnitCost,
                    CreatedAt = movement.TransactionDate
                };

                await _stockRepository.AddStockTransactionAsync(transaction);

                // Update stock summary
                await UpdateStockSummaryAsync(movement.ProductId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording stock movement for product {ProductId}", movement.ProductId);
                return false;
            }
        }

        public async Task<FIFOSaleResultDto> ProcessFIFOSaleAsync(int productId, decimal quantity, int saleReferenceId)
        {
            var result = new FIFOSaleResultDto
            {
                ProductId = productId,
                QuantitySold = quantity
            };

            try
            {
                // Get available batches (FIFO = First In First Out, so order by oldest first)
                var availableBatches = await _stockRepository.GetAvailableBatchesForProductAsync(productId);

                if (!availableBatches.Any())
                {
                    result.Success = false;
                    result.Message = "No available stock for this product";
                    return result;
                }

                var totalAvailable = availableBatches.Sum(b => b.RemainingQty);
                if (totalAvailable < quantity)
                {
                    result.Success = false;
                    result.Message = $"Insufficient stock. Available: {totalAvailable}, Requested: {quantity}";
                    return result;
                }

                decimal remainingQuantity = quantity;
                decimal totalCost = 0;
                var layersUsed = new List<FIFOLayerDto>();

                // Process FIFO - use the oldest batches first
                foreach (var batch in availableBatches.OrderBy(b => b.Purchase?.PurchaseDate))
                {
                    if (remainingQuantity <= 0) break;

                    decimal quantityToUse = Math.Min(batch.RemainingQty, remainingQuantity);
                    decimal layerCost = quantityToUse * batch.UnitCost;

                    layersUsed.Add(new FIFOLayerDto
                    {
                        PurchaseDetailId = batch.PurchaseDetailId,
                        QuantityUsed = quantityToUse,
                        UnitCost = batch.UnitCost,
                        TotalCost = layerCost
                    });

                    totalCost += layerCost;
                    remainingQuantity -= quantityToUse;

                    // Update batch remaining quantity
                    batch.RemainingQty -= quantityToUse;
                    // You'll need to add an UpdatePurchaseDetailAsync method to your repository
                    // await _stockRepository.UpdatePurchaseDetailAsync(batch);
                }

                // Record the sale transaction
                var saleTransaction = new StockTransaction
                {
                    TransactionId = saleReferenceId,
                    ProductId = productId,
                    TransactionType = 'S', // Sale
                    ReferenceId = saleReferenceId,
                    Quantity = -quantity, // Negative for sales
                    UnitCost = totalCost / quantity, // Average cost for the sale
                    TotalCost = totalCost,
                    CreatedAt = DateTime.Now
                };

                await _stockRepository.AddStockTransactionAsync(saleTransaction);

                // Update stock summary
                await UpdateStockSummaryAsync(productId);

                result.Success = true;
                result.TotalCost = totalCost;
                result.LayersUsed = layersUsed;
                result.Message = $"Successfully processed FIFO sale of {quantity} units";

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing FIFO sale for product {ProductId}", productId);
                result.Success = false;
                result.Message = $"Error processing sale: {ex.Message}";
                return result;
            }
        }

        public async Task<decimal> GetProductStockBalanceAsync(int productId)
        {
            try
            {
                var summary = await _stockRepository.GetStockSummaryByProductIdAsync(productId);
                return summary?.Balance ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock balance for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<decimal> GetProductStockValueAsync(int productId)
        {
            try
            {
                var summary = await _stockRepository.GetStockSummaryByProductIdAsync(productId);
                return summary?.TotalValue ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock value for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> UpdateStockSummaryAsync(int productId)
        {
            try
            {
                var transactions = await _stockRepository.GetStockTransactionsByProductAsync(productId);
                var summary = await _stockRepository.GetStockSummaryByProductIdAsync(productId);

                if (summary == null)
                {
                    summary = new StockSummary
                    {
                        ProductId = productId,
                        LastUpdated = DateTime.Now
                    };
                }

                // Calculate totals from transactions
                summary.TotalIn = transactions.Where(t => t.Quantity > 0).Sum(t => t.Quantity);
                summary.TotalOut = Math.Abs(transactions.Where(t => t.Quantity < 0).Sum(t => t.Quantity));
                summary.Balance = summary.TotalIn - summary.TotalOut;

                // Calculate weighted average cost
                var purchaseTransactions = transactions.Where(t => t.Quantity > 0).ToList();
                if (purchaseTransactions.Any())
                {
                    decimal totalCost = purchaseTransactions.Sum(t => t.TotalCost);
                    decimal totalQuantity = purchaseTransactions.Sum(t => t.Quantity);
                    summary.AverageCost = totalQuantity > 0 ? totalCost / totalQuantity : 0;
                }
                else
                {
                    summary.AverageCost = 0;
                }

                // Calculate total value
                summary.TotalValue = summary.Balance * summary.AverageCost;
                summary.LastUpdated = DateTime.Now;

                await _stockRepository.UpdateStockSummaryAsync(summary);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock summary for product {ProductId}", productId);
                return false;
            }
        }


        

        public async Task RestoreStockFromSaleDetailAsync(SaleDetail saleDetail)
        {
            // Restore stock to the exact batches that were consumed
            foreach (var batchUsage in saleDetail.UsedBatches)
            {
                var batch = await _stockRepository.GetPurchaseDetailByIdAsync(
                    batchUsage.PurchaseDetailId);

                if (batch != null)
                {
                    batch.RemainingQty += batchUsage.QuantityUsed;
                    await _stockRepository.UpdatePurchaseDetailAsync(batch);
                }
            }

            // Record adjustment transaction
            var transaction = new StockTransaction
            {
                ProductId = saleDetail.ProductId,
                TransactionType = 'A', // Adjustment (return)
                ReferenceId = saleDetail.SaleDetailId,
                Quantity = saleDetail.Quantity, // Positive (adding back)
                UnitCost = saleDetail.UnitCost,
                TotalCost = saleDetail.TotalCost,
                CreatedAt = DateTime.Now
            };

            await _stockRepository.AddStockTransactionAsync(transaction);
            await UpdateStockSummaryAsync(saleDetail.ProductId);
        }

        public async Task<StockValidationResult> ValidateStockAvailabilityAsync(
            List<(int ProductId, decimal Quantity)> items)
        {
            var result = new StockValidationResult { IsValid = true };

            foreach (var (productId, quantity) in items)
            {
                var balance = await GetProductStockBalanceAsync(productId);
                result.AvailableStock[productId] = balance;

                if (balance < quantity)
                {
                    result.IsValid = false;
                    var product = await _productRepository.GetByIdAsync(productId);
                    result.Errors.Add(
                        $"Product '{product?.Name ?? productId.ToString()}': " +
                        $"Insufficient stock. Available: {balance}, Requested: {quantity}");
                }
            }

            return result;
        }

        // ✅ NEW: Process FIFO with batch tracking
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
                var batches = await _stockRepository.GetAvailableBatchesForProductAsync(productId);

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
                    await _stockRepository.UpdatePurchaseDetailAsync(batch);
                }

                // Record stock transaction
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

                await _stockRepository.AddStockTransactionAsync(transaction);
                await UpdateStockSummaryAsync(productId);

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

        public async Task RestoreStockFromSaleDetailAsync(int saleDetailId)
        {
            try
            {
                _logger.LogInformation(
                    "Starting stock restoration for SaleDetail {SaleDetailId}",
                    saleDetailId);

                // ✅ Get sale detail with batch information
                var saleDetail = await _saleRepository.GetSaleDetailWithBatchesAsync(saleDetailId);

                if (saleDetail == null)
                {
                    throw new InvalidOperationException(
                        $"SaleDetail {saleDetailId} not found");
                }

                if (!saleDetail.UsedBatches.Any())
                {
                    _logger.LogWarning(
                        "SaleDetail {SaleDetailId} has no batch usage records. " +
                        "Cannot restore stock to specific batches.",
                        saleDetailId);

                    // Fallback: Create adjustment without specific batch tracking
                    await CreateStockAdjustmentAsync(
                        saleDetail.ProductId,
                        saleDetail.Quantity,
                        saleDetail.UnitCost,
                        $"Return from SaleDetail {saleDetailId} - No batch info");
                    return;
                }

                _logger.LogInformation(
                    "Restoring {Quantity} units of Product {ProductId} to {BatchCount} batch(es)",
                    saleDetail.Quantity,
                    saleDetail.ProductId,
                    saleDetail.UsedBatches.Count);

                // ✅ Restore stock to the exact batches that were consumed (Reverse FIFO)
                foreach (var batchUsage in saleDetail.UsedBatches)
                {
                    var batch = await _stockRepository.GetPurchaseDetailByIdAsync(
                        batchUsage.PurchaseDetailId);

                    if (batch == null)
                    {
                        _logger.LogWarning(
                            "PurchaseDetail {PurchaseDetailId} not found for restoration. Skipping.",
                            batchUsage.PurchaseDetailId);
                        continue;
                    }

                    // Restore quantity to the batch
                    var oldRemaining = batch.RemainingQty;
                    batch.RemainingQty += batchUsage.QuantityUsed;

                    // Validate we don't exceed original quantity
                    if (batch.RemainingQty > batch.Quantity)
                    {
                        _logger.LogError(
                            "Restoration would exceed original quantity for batch {BatchId}. " +
                            "Original: {Original}, Current: {Current}, Restoring: {Restoring}",
                            batch.PurchaseDetailId,
                            batch.Quantity,
                            oldRemaining,
                            batchUsage.QuantityUsed);

                        // Cap at original quantity
                        batch.RemainingQty = batch.Quantity;
                    }

                    await _stockRepository.UpdatePurchaseDetailAsync(batch);

                    _logger.LogDebug(
                        "Restored {Quantity} to batch {BatchId}. " +
                        "RemainingQty: {OldQty} → {NewQty}",
                        batchUsage.QuantityUsed,
                        batch.PurchaseDetailId,
                        oldRemaining,
                        batch.RemainingQty);
                }

                // ✅ Record adjustment transaction (positive = adding back)
                var transaction = new StockTransaction
                {
                    ProductId = saleDetail.ProductId,
                    TransactionType = 'A', // Adjustment (return)
                    ReferenceId = saleDetailId,
                    Quantity = saleDetail.Quantity, // Positive (adding back)
                    UnitCost = saleDetail.UnitCost,
                    TotalCost = saleDetail.TotalCost,
                    CreatedAt = DateTime.Now
                };

                await _stockRepository.AddStockTransactionAsync(transaction);

                // ✅ Update stock summary
                await UpdateStockSummaryAsync(saleDetail.ProductId);

                _logger.LogInformation(
                    "Successfully restored stock for SaleDetail {SaleDetailId}. " +
                    "Product: {ProductId}, Quantity: {Quantity}",
                    saleDetailId,
                    saleDetail.ProductId,
                    saleDetail.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error restoring stock for SaleDetail {SaleDetailId}",
                    saleDetailId);
                throw;
            }
        }

        /// <summary>
        /// Fallback method to create stock adjustment without batch tracking
        /// </summary>
        private async Task CreateStockAdjustmentAsync(
            int productId,
            decimal quantity,
            decimal unitCost,
            string notes)
        {
            var transaction = new StockTransaction
            {
                ProductId = productId,
                TransactionType = 'A',
                ReferenceId = 0,
                Quantity = quantity,
                UnitCost = unitCost,
                TotalCost = quantity * unitCost,
                CreatedAt = DateTime.Now
            };

            await _stockRepository.AddStockTransactionAsync(transaction);
            await UpdateStockSummaryAsync(productId);

            //_logger.LogWarning(
            //    "Created stock adjustment for Product {ProductId} without batch tracking: {Notes}",
            //    productId, notes);
        }
    }
}