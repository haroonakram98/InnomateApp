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
        private readonly IUnitOfWork _uow;
        private readonly IStockService _stockService;
        private readonly ISequenceService _sequenceService;

        public SaleService(IUnitOfWork uow, IStockService stockService, ISequenceService sequenceService)
        {
            _uow = uow;
            _stockService = stockService;
            _sequenceService = sequenceService;
        }

        public async Task<IEnumerable<SaleResponse>> GetAllAsync()
        {
            var sales = await _uow.Sales.GetSalesWithDetailsAsync();

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
                PaidAmount = s.PaidAmount,
                BalanceAmount = s.BalanceAmount,
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
            var sale = await _uow.Sales.GetSaleWithDetailsAsync(saleId);
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



        public async Task<SaleResponse?> UpdateAsync(UpdateSaleRequest request)
        {
            var sale = await _uow.Sales.GetSaleWithDetailsAsync(request.SaleId);
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

            await _uow.Sales.UpdateAsync(sale);
            await _uow.SaveChangesAsync();

            return await GetByIdAsync(sale.SaleId);
        }

        public async Task<bool> DeleteAsync(int saleId)
        {
            var sale = await _uow.Sales.GetByIdAsync(saleId);
            if (sale == null) return false;

            await _uow.Sales.DeleteAsync(sale);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetNextInvoiceNumberAsync()
        {
            return await _sequenceService.GenerateInvoiceNumberAsync();
        }

        public async Task<SaleResponse> AddPaymentAsync(AddPaymentRequest request)
        {
            await using var transaction = await _uow.BeginTransactionAsync();

            try
            {
                var sale = await _uow.Sales.GetSaleWithDetailsAsync(request.SaleId);
                if (sale == null) throw new KeyNotFoundException("Sale not found");

                if (request.Amount > sale.BalanceAmount)
                {
                    throw new InvalidOperationException($"Payment amount ({request.Amount}) exceeds current balance ({sale.BalanceAmount})");
                }

                var payment = new Payment
                {
                    SaleId = request.SaleId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    ReferenceNo = request.ReferenceNo,
                    PaymentDate = DateTime.Now
                };

                await _uow.Payments.AddAsync(payment);

                // Update Sale summary
                sale.PaidAmount += request.Amount;
                sale.BalanceAmount -= request.Amount;
                if (sale.BalanceAmount <= 0)
                {
                    sale.BalanceAmount = 0;
                    sale.IsFullyPaid = true;
                }

                await _uow.Sales.UpdateAsync(sale);
                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetByIdAsync(sale.SaleId) ?? throw new Exception("Failed to retrieve updated sale");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
