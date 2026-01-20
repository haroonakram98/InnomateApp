using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Common;
using InnomateApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces.Services;

namespace InnomateApp.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PurchaseService> _logger;
        private readonly IValidator<CreatePurchaseDto> _validator;
        private readonly ISequenceService _sequenceService;

        public PurchaseService(
            IUnitOfWork uow,
            ILogger<PurchaseService> logger,
            IValidator<CreatePurchaseDto> validator,
            ISequenceService sequenceService)
        {
            _uow = uow;
            _logger = logger;
            _validator = validator;
            _sequenceService = sequenceService;
        }

        public async Task<Purchase> CreatePurchaseAsync(Purchase purchase)
        {
            await using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                // 1. Validation (SRP: Using FluentValidation)
                // We need to map the Domain Entity back to DTO for validation, 
                // OR ideally, the service should accept the DTO. 
                // Since the interface accepts Entity, we'll map manually for now to enforce rules.
                var purchaseDto = new CreatePurchaseDto
                {
                    SupplierId = purchase.SupplierId,
                    PurchaseDate = purchase.PurchaseDate,
                    InvoiceNo = purchase.InvoiceNo,
                    CreatedBy = purchase.CreatedBy,
                    Notes = purchase.Notes,
                    PurchaseDetails = purchase.PurchaseDetails.Select(d => new CreatePurchaseDetailDto
                    {
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitCost = d.UnitCost,
                        BatchNo = d.BatchNo,
                        ExpiryDate = d.ExpiryDate
                    }).ToList()
                };

                var validationResult = await _validator.ValidateAsync(purchaseDto);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                // 2. Business Logic Validation (Existence checks)
                var supplier = await _uow.Suppliers.GetByIdAsync(purchase.SupplierId);
                if (supplier == null)
                    throw new EntityNotFoundException("Supplier", purchase.SupplierId);

                // Validate products exist
                foreach (var detail in purchase.PurchaseDetails)
                {
                    var product = await _uow.Products.GetByIdAsync(detail.ProductId);
                    if (product == null)
                        throw new EntityNotFoundException("Product", detail.ProductId);
                }

                // Generate purchase number and set initial values
                if (string.IsNullOrWhiteSpace(purchase.InvoiceNo))
                {
                    purchase.InvoiceNo = await _sequenceService.GeneratePurchaseNumberAsync();
                }

                purchase.Status = "Pending";
                purchase.CreatedAt = DateTime.Now;

                // Calculate totals
                purchase.CalculateTotal();

                // Save purchase
                var createdPurchase = await _uow.Purchases.AddAsync(purchase);
                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Purchase {InvoiceNo} created successfully with {DetailCount} items",
                    purchase.InvoiceNo, purchase.PurchaseDetails.Count);

                return createdPurchase;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Purchase> ReceivePurchaseAsync(int purchaseId)
        {
            await using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                var purchase = await _uow.Purchases.GetPurchaseWithDetailsAsync(purchaseId);
                if (purchase == null)
                    throw new EntityNotFoundException("Purchase", purchaseId);

                if (purchase.Status != "Pending")
                    throw new BusinessRuleViolationException($"Cannot receive purchase that is already {purchase.Status}");

                // Update purchase status
                purchase.MarkAsReceived();

                // Process each purchase detail
                foreach (var detail in purchase.PurchaseDetails)
                {
                    // Initialize remaining quantity for FIFO tracking
                    detail.RemainingQty = detail.Quantity;

                    // Update purchase detail
                    await _uow.Purchases.UpdatePurchaseDetailAsync(detail);

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

                    await _uow.Stock.AddStockTransactionAsync(stockTransaction);

                    // Update stock summary
                    await _uow.Stock.UpdateStockSummaryAsync(new StockSummary
                    {
                        ProductId = detail.ProductId,
                        TotalIn = detail.Quantity,
                        Balance = detail.Quantity,
                        AverageCost = detail.UnitCost,
                        TotalValue = detail.TotalCost,
                        LastUpdated = DateTime.Now
                    });
                }

                // Update purchase
                await _uow.Purchases.UpdateAsync(purchase);
                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Purchase {PurchaseNumber} received successfully with {DetailCount} items",
                    purchase.InvoiceNo, purchase.PurchaseDetails.Count);

                return purchase;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelPurchaseAsync(int purchaseId)
        {
            var purchase = await _uow.Purchases.GetByIdAsync(purchaseId);
            if (purchase == null)
                throw new EntityNotFoundException("Purchase",purchaseId);

            if (purchase.Status == "Received")
                throw new BusinessRuleViolationException("Cannot cancel a received purchase. Consider creating a return instead.");

            if (purchase.Status == "Cancelled")
                throw new BusinessRuleViolationException("Purchase is already cancelled");

            purchase.Status = "Cancelled";
            await _uow.Purchases.UpdateAsync(purchase);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Purchase {PurchaseNumber} cancelled successfully", purchase.InvoiceNo);
            return true;
        }

        public async Task<Purchase> GetPurchaseByIdAsync(int purchaseId)
        {
            var purchase = await _uow.Purchases.GetPurchaseWithDetailsAsync(purchaseId);
            if (purchase == null)
                throw new EntityNotFoundException("Purchase",purchaseId);

            return purchase;
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the entire day
            var endDateInclusive = endDate.Date.AddDays(1).AddSeconds(-1);

            var purchases = await _uow.Purchases.GetPurchasesByDateRangeAsync(startDate, endDateInclusive);

            _logger.LogDebug("Retrieved {Count} purchases between {StartDate} and {EndDate}",
                purchases.Count, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            return purchases;
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesWithDetailsAsync()
        {
            var purchases = await _uow.Purchases.GetPurchasesWithDetailsAsync();

            _logger.LogDebug("Retrieved {Count} purchases with details", purchases.Count);

            return purchases;
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId)
        {
            // Validate supplier exists
            var supplier = await _uow.Suppliers.GetByIdAsync(supplierId);
            if (supplier == null)
                throw new InnomateApp.Domain.Common.EntityNotFoundException("Supplier", supplierId);

            var purchases = await _uow.Purchases.GetPurchasesBySupplierAsync(supplierId);

            _logger.LogDebug("Retrieved {Count} purchases for supplier {SupplierId}",
                purchases.Count, supplierId);

            return purchases;
        }

        // Additional helper methods
        public async Task<IReadOnlyList<Purchase>> GetPendingPurchasesAsync()
        {
            var allPurchases = await _uow.Purchases.GetPurchasesWithDetailsAsync();
            var pendingPurchases = allPurchases
                .Where(p => p.Status == "Pending")
                .OrderBy(p => p.PurchaseDate)
                .ToList();

            _logger.LogDebug("Retrieved {Count} pending purchases", pendingPurchases.Count);

            return pendingPurchases;
        }

        public async Task<IReadOnlyList<Purchase>> GetRecentPurchasesAsync(int count = 10)
        {
            var allPurchases = await _uow.Purchases.GetPurchasesWithDetailsAsync();
            var recentPurchases = allPurchases
                .OrderByDescending(p => p.PurchaseDate)
                .Take(count)
                .ToList();

            return recentPurchases;
        }

        public async Task<decimal> GetPurchaseTotalByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await GetPurchasesByDateRangeAsync(startDate, endDate);
            return purchases.Sum(p => p.TotalAmount);
        }

        public async Task<string> GetNextPurchaseNumberAsync()
        {
            return await _sequenceService.GeneratePurchaseNumberAsync();
        }

        public async Task<string> GetNextBatchNumberAsync()
        {
             return await _sequenceService.GenerateBatchNumberAsync();
        }
    }
}