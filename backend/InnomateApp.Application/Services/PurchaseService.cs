using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IStockSummaryRepository _stockSummaryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(
            IPurchaseRepository purchaseRepository,
            IStockTransactionRepository stockTransactionRepository,
            IStockSummaryRepository stockSummaryRepository,
            ISupplierRepository supplierRepository,
            IProductRepository productRepository,
            ILogger<PurchaseService> logger)
        {
            _purchaseRepository = purchaseRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _stockSummaryRepository = stockSummaryRepository;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Purchase> CreatePurchaseAsync(Purchase purchase)
        {
            try
            {
                // Validate supplier exists
                var supplier = await _supplierRepository.GetByIdAsync(purchase.SupplierId);
                if (supplier == null)
                    throw new ArgumentException($"Supplier with ID {purchase.SupplierId} not found");

                // Validate products exist
                foreach (var detail in purchase.PurchaseDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product == null)
                        throw new ArgumentException($"Product with ID {detail.ProductId} not found");
                }

                // Generate purchase number and set initial values
                if (string.IsNullOrWhiteSpace(purchase.InvoiceNo))
                {
                    purchase.InvoiceNo = await GeneratePurchaseNumberAsync();
                }
                // Optionally validate: if (purchase.InvoiceNo is not valid format) ... 
                purchase.Status = "Pending";
                purchase.CreatedAt = DateTime.Now;

                // Calculate totals
                purchase.CalculateTotal();

                // Save purchase
                var createdPurchase = await _purchaseRepository.AddAsync(purchase);

                _logger.LogInformation("Purchase {InvoiceNo} created successfully with {DetailCount} items",
                    purchase.InvoiceNo, purchase.PurchaseDetails.Count);

                return createdPurchase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase for supplier {SupplierId}", purchase.SupplierId);
                throw;
            }
        }

        public async Task<Purchase> ReceivePurchaseAsync(int purchaseId)
        {
            try
            {
                var purchase = await _purchaseRepository.GetPurchaseWithDetailsAsync(purchaseId);
                if (purchase == null)
                    throw new ArgumentException($"Purchase with ID {purchaseId} not found");

                if (purchase.Status != "Pending")
                    throw new InvalidOperationException($"Cannot receive purchase that is already {purchase.Status}");

                // Update purchase status
                purchase.MarkAsReceived();

                // Process each purchase detail
                foreach (var detail in purchase.PurchaseDetails)
                {
                    // Initialize remaining quantity for FIFO tracking
                    detail.RemainingQty = detail.Quantity;

                    // Update purchase detail
                    await _purchaseRepository.UpdatePurchaseDetailAsync(detail);

                    // Create stock transaction
                    var stockTransaction = new StockTransaction
                    {
                        ProductId = detail.ProductId,
                        TransactionType = 'I',
                        Quantity = detail.Quantity,
                        UnitCost = detail.UnitCost,
                        TotalCost = detail.TotalCost,
                        CreatedAt = DateTime.Now,
                        Reference = $"PUR-{purchase.InvoiceNo}",
                        Notes = $"Purchase receipt - {purchase.InvoiceNo}",
                        ReferenceId = purchase.PurchaseId
                    };

                    await _stockTransactionRepository.AddAsync(stockTransaction);

                    // Update stock summary
                    await _stockSummaryRepository.UpdateStockSummaryAsync(
                        detail.ProductId,
                        detail.Quantity,
                        detail.UnitCost);

                    _logger.LogDebug("Updated stock for product {ProductId}, quantity: {Quantity}",
                        detail.ProductId, detail.Quantity);
                }

                // Update purchase
                await _purchaseRepository.UpdateAsync(purchase);

                _logger.LogInformation("Purchase {PurchaseNumber} received successfully with {DetailCount} items",
                    purchase.InvoiceNo, purchase.PurchaseDetails.Count);

                return purchase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving purchase {PurchaseId}", purchaseId);
                throw;
            }
        }

        public async Task<bool> CancelPurchaseAsync(int purchaseId)
        {
            try
            {
                var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);
                if (purchase == null)
                    throw new ArgumentException($"Purchase with ID {purchaseId} not found");

                if (purchase.Status == "Received")
                    throw new InvalidOperationException("Cannot cancel a received purchase. Consider creating a return instead.");

                if (purchase.Status == "Cancelled")
                    throw new InvalidOperationException("Purchase is already cancelled");

                purchase.Status = "Cancelled";
                await _purchaseRepository.UpdateAsync(purchase);

                _logger.LogInformation("Purchase {PurchaseNumber} cancelled successfully", purchase.InvoiceNo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling purchase {PurchaseId}", purchaseId);
                throw;
            }
        }

        public async Task<Purchase> GetPurchaseByIdAsync(int purchaseId)
        {
            try
            {
                var purchase = await _purchaseRepository.GetPurchaseWithDetailsAsync(purchaseId);
                if (purchase == null)
                    throw new ArgumentException($"Purchase with ID {purchaseId} not found");

                return purchase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase {PurchaseId}", purchaseId);
                throw;
            }
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Ensure end date includes the entire day
                var endDateInclusive = endDate.Date.AddDays(1).AddSeconds(-1);

                var purchases = await _purchaseRepository.GetPurchasesByDateRangeAsync(startDate, endDateInclusive);

                _logger.LogDebug("Retrieved {Count} purchases between {StartDate} and {EndDate}",
                    purchases.Count, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

                return purchases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases by date range {StartDate} to {EndDate}",
                    startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                throw;
            }
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesWithDetailsAsync()
        {
            try
            {
                var purchases = await _purchaseRepository.GetPurchasesWithDetailsAsync();

                _logger.LogDebug("Retrieved {Count} purchases with details", purchases.Count);

                return purchases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all purchases with details");
                throw;
            }
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId)
        {
            try
            {
                // Validate supplier exists
                var supplier = await _supplierRepository.GetByIdAsync(supplierId);
                if (supplier == null)
                    throw new ArgumentException($"Supplier with ID {supplierId} not found");

                var purchases = await _purchaseRepository.GetPurchasesBySupplierAsync(supplierId);

                _logger.LogDebug("Retrieved {Count} purchases for supplier {SupplierId}",
                    purchases.Count, supplierId);

                return purchases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases for supplier {SupplierId}", supplierId);
                throw;
            }
        }

        // Additional helper methods
        public async Task<IReadOnlyList<Purchase>> GetPendingPurchasesAsync()
        {
            try
            {
                var allPurchases = await _purchaseRepository.GetPurchasesWithDetailsAsync();
                var pendingPurchases = allPurchases
                    .Where(p => p.Status == "Pending")
                    .OrderBy(p => p.PurchaseDate)
                    .ToList();

                _logger.LogDebug("Retrieved {Count} pending purchases", pendingPurchases.Count);

                return pendingPurchases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending purchases");
                throw;
            }
        }

        public async Task<IReadOnlyList<Purchase>> GetRecentPurchasesAsync(int count = 10)
        {
            try
            {
                var allPurchases = await _purchaseRepository.GetPurchasesWithDetailsAsync();
                var recentPurchases = allPurchases
                    .OrderByDescending(p => p.PurchaseDate)
                    .Take(count)
                    .ToList();

                return recentPurchases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent purchases");
                throw;
            }
        }

        public async Task<decimal> GetPurchaseTotalByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var purchases = await GetPurchasesByDateRangeAsync(startDate, endDate);
                return purchases.Sum(p => p.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating purchase total for date range");
                throw;
            }
        }

        public async Task<string> GetNextPurchaseNumberAsync()
        {
            return await GeneratePurchaseNumberAsync();
        }

        public async Task<string> GetNextBatchNumberAsync()
        {
             // Generate a concise unique batch number: BN-{yyyyMMdd}-{HHmmss}-{Random}
             // Or simpler: BN-{yyyyMMdd}-{Random4Chars} to keep it short but unique enough for UI
             var today = DateTime.Now.ToString("yyyyMMdd");
             var random = new Random();
             var randSuffix = random.Next(1000, 9999);
             return $"BN-{today}-{randSuffix}";
        }

        private async Task<string> GeneratePurchaseNumberAsync()
        {
            try
            {
                var today = DateTime.Now.ToString("yyyyMMdd");

                // Get today's purchases to count
                var startOfDay = DateTime.Now.Date;
                var endOfDay = startOfDay.AddDays(1);

                var todaysPurchases = await _purchaseRepository.GetPurchasesByDateRangeAsync(startOfDay, endOfDay);
                var count = todaysPurchases.Count;

                var purchaseNumber = $"PUR-{today}-{count + 1:0000}";

                _logger.LogDebug("Generated purchase number: {PurchaseNumber}", purchaseNumber);

                return purchaseNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating purchase number");
                // Fallback purchase number
                return $"PUR-{DateTime.Now:yyyyMMdd-HHmmss}";
            }
        }
    }
}