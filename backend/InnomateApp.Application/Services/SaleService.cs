using InnomateApp.Application.DTOs.Sales.Requests;
using InnomateApp.Application.DTOs.Sales.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Application.Interfaces.Services;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly IGenericRepository<SaleDetail> _saleDetailRepo;
        private readonly IGenericRepository<Payment> _paymentRepo;
        private readonly IPurchaseRepository _purchaseRepo;
        private readonly IStockRepository _stockRepo;


        public SaleService(
            ISaleRepository saleRepo,
            IGenericRepository<SaleDetail> saleDetailRepo,
            IGenericRepository<Payment> paymentRepo,
            IPurchaseRepository purchaseRepo,
            IStockRepository stockRepo)
        {
            _saleRepo = saleRepo;
            _saleDetailRepo = saleDetailRepo;
            _paymentRepo = paymentRepo;
            _purchaseRepo = purchaseRepo;
            _stockRepo = stockRepo;
        }

        public async Task<IEnumerable<SaleResponse>> GetAllAsync()
        {
            var sales = await _saleRepo.GetSalesWithDetailsAsync();

            return sales.Select(s => new SaleResponse
            {
                SaleId = s.SaleId,
                SaleDate = s.SaleDate,
                CustomerId = s.CustomerId,
                InvoiceNo = s.InvoiceNo,
                TotalAmount = s.TotalAmount,
                CreatedAt = s.CreatedAt,
                SubTotal = s.SubTotal,
                CreatedBy = s.CreatedBy,
                TotalProfit = s.TotalProfit,
                ProfitMargin = s.ProfitMargin,
                IsFullyPaid = s.IsFullyPaid,
                Customer = s.Customer == null ? null : new CustomerShortResponse
                {
                    CustomerId = s.Customer.CustomerId,
                    Name = s.Customer.Name,
                    Phone = s.Customer.Phone
                },
                SaleDetails = s.SaleDetails.Select(d => new SaleDetailResponse
                {
                    SaleDetailId = d.SaleDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Total = d.Total
                }).ToList(),
                Payments = s.Payments.Select(p => new PaymentResponse
                {
                    PaymentId = p.PaymentId,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    ReferenceNo = p.ReferenceNo
                }).ToList()
            }).ToList();
        }

        public async Task<SaleResponse?> GetByIdAsync(int saleId)
        {
            var sale = await _saleRepo.GetSaleWithDetailsAsync(saleId);
            if (sale == null) return null;

            return new SaleResponse
            {
                SaleId = sale.SaleId,
                SaleDate = sale.SaleDate,
                CustomerId = sale.CustomerId,
                InvoiceNo = sale.InvoiceNo,
                TotalAmount = sale.TotalAmount,
                CreatedAt = sale.CreatedAt,
                Customer = sale.Customer == null ? null : new CustomerShortResponse
                {
                    CustomerId = sale.Customer.CustomerId,
                    Name = sale.Customer.Name,
                    Phone = sale.Customer.Phone
                },
                SaleDetails = sale.SaleDetails.Select(d => new SaleDetailResponse
                {
                    SaleDetailId = d.SaleDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Total = d.Total
                }).ToList(),
                Payments = sale.Payments.Select(p => new PaymentResponse
                {
                    PaymentId = p.PaymentId,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    ReferenceNo = p.ReferenceNo
                }).ToList()
            };
        }

        public async Task<SaleResponse> CreateAsync(CreateSaleRequest request)
        {
            var productIds = request.SaleDetails.Select(d => d.ProductId).Distinct().ToList();
            var productCosts = await _purchaseRepo.GetProductCostsAsync(productIds);

            // --- 1. First create the sale to get SaleId ---
            var sale = new Sale
            {
                CustomerId = (request.CustomerId == 0) ? null : request.CustomerId,
                InvoiceNo = request.InvoiceNo,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.Now,
                SaleDate = DateTime.Now,
                PaidAmount = request.PaidAmount,
                BalanceAmount = request.BalanceAmount,
                IsFullyPaid = request.IsFullyPaid,
                DiscountType = request.DiscountType,
                DiscountPercentage = request.DiscountPercentage,
                Discount = request.Discount,
                SaleDetails = request.SaleDetails.Select(d => new SaleDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Total = d.Quantity * d.UnitPrice,
                    Discount = d.Discount,
                    DiscountPercentage = d.DiscountPercentage,
                    DiscountType = d.DiscountType,
                    NetAmount = (d.Quantity * d.UnitPrice) - d.Discount
                }).ToList(),
                Payments = request.Payments.Select(p => new Payment
                {
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    ReferenceNo = p.ReferenceNo,
                    PaymentDate = DateTime.Now
                }).ToList()
            };

            sale.TotalAmount = sale.SaleDetails.Sum(x => x.Total) - sale.Discount;
            sale.SubTotal = sale.SaleDetails.Sum(x => x.Total);
            sale.TotalCost = sale.SaleDetails.Sum(d => d.Quantity * productCosts[d.ProductId]);
            sale.TotalProfit = sale.TotalAmount - sale.TotalCost;
            sale.ProfitMargin = sale.TotalAmount > 0 ? (sale.TotalProfit / sale.TotalAmount) * 100 : 0;

            await _saleRepo.AddAsync(sale);

            // --- 2. Now process stock transactions with the SaleId ---
            foreach (var detail in request.SaleDetails)
            {
                int productId = detail.ProductId;
                decimal saleQty = detail.Quantity;

                // --- 3.1 Validate stock summary ---
                var stock = await _stockRepo.GetStockSummaryByProductIdAsync(productId);
                if (stock == null)
                    throw new Exception($"Stock summary not found for product {productId}");

                if (stock.Balance < saleQty)
                    throw new Exception($"Insufficient stock for product {productId}. Available: {stock.Balance}, Required: {saleQty}");

                // --- 3.2 FIFO Batch Deduction ---
                decimal qtyToDeduct = saleQty;
                var batches = await _stockRepo.GetAvailableBatchesForProductAsync(productId);

                foreach (var batch in batches)
                {
                    if (qtyToDeduct <= 0) break;

                    decimal deductQty = Math.Min(batch.RemainingQty, qtyToDeduct);

                    // Deduct from batch
                    batch.RemainingQty -= deductQty;
                    qtyToDeduct -= deductQty;

                    // Update PurchaseDetail
                    await _stockRepo.UpdatePurchaseDetailAsync(batch);

                    // Create Stock Transaction with SaleId as ReferenceId
                    await _stockRepo.AddStockTransactionAsync(new StockTransaction
                    {
                        ProductId = productId,
                        TransactionType = 'O', // 'O' for Out/ Sale
                        Quantity = -deductQty,
                        UnitCost = batch.UnitCost,
                        TotalCost = batch.TotalCost,
                        CreatedAt = DateTime.Now,
                        Reference = $"INV-{request.InvoiceNo}",
                        Notes = $"INV # - {request.InvoiceNo}",
                        ReferenceId = sale.SaleId, // Add SaleId as ReferenceId
                    });
                }

                // If still not enough stock
                if (qtyToDeduct > 0)
                    throw new Exception($"FIFO batches insufficient for product {productId}");

                // --- 3.3 Update Stock Summary ---
                stock.TotalOut += saleQty;
                stock.Balance -= saleQty;
                await _stockRepo.UpdateStockSummaryAsync(stock);
            }

            // Update the sale with calculated costs (if needed)
            // You might want to update the sale after stock processing if there were any changes
            await _saleRepo.UpdateAsync(sale); // Optional: if you need to update sale after stock processing

            var created = await _saleRepo.GetSaleWithDetailsAsync(sale.SaleId);
            return await GetByIdAsync(created!.SaleId) ?? throw new Exception("Creation failed");
        }

        public async Task<SaleResponse?> UpdateAsync(UpdateSaleRequest request)
        {
            var sale = await _saleRepo.GetSaleWithDetailsAsync(request.SaleId);
            if (sale == null) return null;

            sale.InvoiceNo = request.InvoiceNo;
            sale.CustomerId = (request.CustomerId == 0) ? null : request.CustomerId;

            // Remove old details & payments
            sale.SaleDetails.Clear();
            sale.Payments.Clear();

            // Add new ones
            sale.SaleDetails = request.SaleDetails.Select(d => new SaleDetail
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Total = d.Quantity * d.UnitPrice,
                Discount = d.Discount,
                DiscountPercentage = d.DiscountPercentage,
                DiscountType = d.DiscountType,
                NetAmount = (d.Quantity * d.UnitPrice) - d.Discount
            }).ToList();

            sale.Payments = request.Payments.Select(p => new Payment
            {
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                ReferenceNo = p.ReferenceNo
            }).ToList();

            sale.TotalAmount = sale.SaleDetails.Sum(x => x.Total);

            await _saleRepo.UpdateAsync(sale);

            return await GetByIdAsync(sale.SaleId);
        }

        public async Task<bool> DeleteAsync(int saleId)
        {
            var sale = await _saleRepo.GetByIdAsync(saleId);
            if (sale == null) return false;

            await _saleRepo.UpdateAsync(sale);
            await _saleRepo.UpdateAsync(sale);
            return true;
        }

        public async Task<string> GetNextInvoiceNumberAsync()
        {
            var lastInvoiceNo = await _saleRepo.GetLastInvoiceNoAsync();

            if (string.IsNullOrEmpty(lastInvoiceNo))
            {
                return "INV-1001";
            }

            // Expected format: INV-XXXX
            var parts = lastInvoiceNo.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1], out int number))
            {
                return $"INV-{number + 1}";
            }

            // Fallback if format is weird
            return $"INV-{DateTime.Now.Ticks}";
        }
    }
}
