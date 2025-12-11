using InnomateApp.Application.DTOs.Returns.Requests;
using InnomateApp.Application.DTOs.Returns.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Application.Interfaces.Services;
using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class ReturnService : IReturnService
    {
        private readonly IReturnRepository _returnRepo;
        private readonly ISaleRepository _saleRepo;
        private readonly IStockService _stockService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockRepository _stockRepo;

        // Constructor handles dependency injection
        public ReturnService(
            IReturnRepository returnRepo, 
            ISaleRepository saleRepo, 
            IStockService stockService,
            IStockRepository stockRepo,
            IUnitOfWork unitOfWork)
        {
            _returnRepo = returnRepo;
            _saleRepo = saleRepo;
            _stockService = stockService;
            _stockRepo = stockRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReturnResponse> CreateReturnAsync(CreateReturnRequest request)
        {
            // ============================================================
            // 1. Load Sale
            // ============================================================
            var sale = await _saleRepo.GetSaleWithDetailsAsync(request.SaleId);
            if (sale == null)
                throw new Exception($"Sale with ID {request.SaleId} not found.");

            // ============================================================
            // 2. Validate previous returns (no over-return)
            // ============================================================
            var existingReturns = await _returnRepo.GetReturnsBySaleIdAsync(sale.SaleId);

            var returnDetails = new List<ReturnDetail>();
            decimal totalRefund = 0;

            foreach (var item in request.ReturnDetails)
            {
                var saleDetail = sale.SaleDetails.FirstOrDefault(sd => sd.ProductId == item.ProductId);
                if (saleDetail == null)
                    throw new Exception($"Product {item.ProductId} not found in sale {sale.SaleId}");

                // How much already returned for this product?
                var prevReturned = existingReturns
                    .SelectMany(r => r.ReturnDetails)
                    .Where(rd => rd.ProductId == item.ProductId)
                    .Sum(rd => rd.Quantity);

                if (item.Quantity + prevReturned > saleDetail.Quantity)
                    throw new Exception(
                        $"Cannot return {item.Quantity} of product {item.ProductId}. " +
                        $"Sold: {saleDetail.Quantity}, Already Returned: {prevReturned}");

                // Refund calculation
                var lineRefund = item.Quantity * saleDetail.UnitPrice;
                totalRefund += lineRefund;

                returnDetails.Add(new ReturnDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = saleDetail.UnitPrice,
                    Total = lineRefund
                });
            }


            // ============================================================
            // 3. STOCK REVERSAL (FIFO Reverse)
            // ============================================================
            foreach (var item in request.ReturnDetails)
            {
                int productId = item.ProductId;
                decimal returnQty = item.Quantity;

                // ---- 3.1 Fetch Stock Summary ----
                var stock = await _stockRepo.GetStockSummaryByProductIdAsync(productId);
                if (stock == null)
                    throw new Exception($"Stock summary not found for product {productId}");

                // ---- 3.2 FIFO reverse: fill batches based on oldest purchase first ----
                decimal qtyToReturn = returnQty;
                var batches = await _stockRepo.GetPurchaseDetailsForReturnFIFOAsync(productId);
                // Make this method: SELECT PurchaseDetails ORDER BY PurchaseDate, PurchaseDetailId

                foreach (var batch in batches)
                {
                    if (qtyToReturn <= 0) break;

                    // Max we can restore is: OriginalQty - RemainingQty
                    decimal consumed = batch.Quantity - batch.RemainingQty;
                    if (consumed <= 0) continue; // This batch wasn't used

                    decimal restoreQty = Math.Min(consumed, qtyToReturn);

                    // Reverse consumption
                    batch.RemainingQty += restoreQty;
                    qtyToReturn -= restoreQty;

                    // Update batch
                    await _stockRepo.UpdatePurchaseDetailAsync(batch);

                    // Create Stock Transaction entry
                    await _stockRepo.AddStockTransactionAsync(new StockTransaction
                    {
                        ProductId = productId,
                        TransactionType = 'R',  // A = Adjustment (Return)
                        Quantity = restoreQty,
                        UnitCost = batch.UnitCost,
                        TotalCost = restoreQty * batch.UnitCost,
                        CreatedAt = DateTime.Now,
                        Reference = $"RET-{sale.InvoiceNo}",
                        Notes = $"Return of INV-{sale.InvoiceNo}",
                        ReferenceId = sale.SaleId
                    });
                }

                if (qtyToReturn > 0)
                    throw new Exception($"FIFO restore failed for product {productId}. Stock mismatch.");

                // ---- 3.3 Update Stock Summary ----
                stock.TotalOut -= returnQty;
                stock.Balance += returnQty;

                await _stockRepo.UpdateStockSummaryAsync(stock);
            }


            // ============================================================
            // 4. Create Return Record
            // ============================================================
            var returnEntity = new Return
            {
                SaleId = sale.SaleId,
                ReturnDate = DateTime.Now,
                Reason = request.Reason,
                TotalRefund = totalRefund,
                ReturnDetails = returnDetails
            };

            await _returnRepo.AddAsync(returnEntity);
            await _unitOfWork.SaveChangesAsync();


            // ============================================================
            // 5. Build Response
            // ============================================================
            return new ReturnResponse
            {
                ReturnId = returnEntity.ReturnId,
                SaleId = sale.SaleId,
                ReturnDate = returnEntity.ReturnDate,
                TotalRefund = returnEntity.TotalRefund,
                Reason = returnEntity.Reason,
                ReturnDetails = returnEntity.ReturnDetails.Select(rd => new ReturnDetailResponse
                {
                    ProductId = rd.ProductId,
                    Quantity = rd.Quantity,
                    RefundAmount = rd.Total,
                    ProductName = sale.SaleDetails
                        .First(d => d.ProductId == rd.ProductId)
                        .Product?.Name
                }).ToList()
            };
        }
        public async Task<IEnumerable<ReturnResponse>> GetReturnsBySaleIdAsync(int saleId)
        {
            var returns = await _returnRepo.GetReturnsBySaleIdAsync(saleId);
            return returns.Select(r => new ReturnResponse
            {
                ReturnId = r.ReturnId,
                SaleId = r.SaleId,
                ReturnDate = r.ReturnDate,
                TotalRefund = r.TotalRefund,
                Reason = r.Reason,
                ReturnDetails = r.ReturnDetails.Select(rd => new ReturnDetailResponse
                {
                    ProductId = rd.ProductId,
                    Quantity = rd.Quantity,
                    RefundAmount = rd.Total,
                    ProductName = rd.Product?.Name
                }).ToList()
            }).ToList();
        }

        public async Task<ReturnResponse?> GetReturnByIdAsync(int returnId)
        {
            var r = await _returnRepo.GetReturnWithDetailsAsync(returnId);
            if (r == null) return null;

            return new ReturnResponse
            {
                ReturnId = r.ReturnId,
                SaleId = r.SaleId,
                ReturnDate = r.ReturnDate,
                TotalRefund = r.TotalRefund,
                Reason = r.Reason,
                ReturnDetails = r.ReturnDetails.Select(rd => new ReturnDetailResponse
                {
                    ProductId = rd.ProductId,
                    Quantity = rd.Quantity,
                    RefundAmount = rd.Total,
                    ProductName = rd.Product?.Name
                }).ToList()
            };
        }
    }
}
