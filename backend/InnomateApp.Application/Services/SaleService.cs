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

        public SaleService(
            ISaleRepository saleRepo,
            IGenericRepository<SaleDetail> saleDetailRepo,
            IGenericRepository<Payment> paymentRepo)
        {
            _saleRepo = saleRepo;
            _saleDetailRepo = saleDetailRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<IEnumerable<SaleResponse>> GetAllAsync()
        {
            var sales = await _saleRepo.GetAllWithDetailsAsync();

            return sales.Select(s => new SaleResponse
            {
                SaleId = s.SaleId,
                SaleDate = s.SaleDate,
                CustomerId = s.CustomerId,
                InvoiceNo = s.InvoiceNo,
                TotalAmount = s.TotalAmount,
                CreatedAt = s.CreatedAt,
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
            var sale = await _saleRepo.GetByIdWithDetailsAsync(saleId);
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
            var sale = new Sale
            {
                CustomerId = request.CustomerId,
                InvoiceNo = request.InvoiceNo,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                SaleDate = DateTime.UtcNow,
                SaleDetails = request.SaleDetails.Select(d => new SaleDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Total = d.Quantity * d.UnitPrice,
                    PurchaseDetailId = d.PurchaseDetailId
                }).ToList(),
                Payments = request.Payments.Select(p => new Payment
                {
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    ReferenceNo = p.ReferenceNo,
                    PaymentDate = DateTime.UtcNow
                }).ToList()
            };

            sale.TotalAmount = sale.SaleDetails.Sum(x => x.Total);
            await _saleRepo.AddAsync(sale);
            await _saleRepo.SaveChangesAsync();

            var created = await _saleRepo.GetByIdWithDetailsAsync(sale.SaleId);
            return await GetByIdAsync(created!.SaleId) ?? throw new Exception("Creation failed");
        }

        public async Task<SaleResponse?> UpdateAsync(UpdateSaleRequest request)
        {
            var sale = await _saleRepo.GetByIdWithDetailsAsync(request.SaleId);
            if (sale == null) return null;

            sale.InvoiceNo = request.InvoiceNo;
            sale.CustomerId = request.CustomerId;

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
                PurchaseDetailId = d.PurchaseDetailId
            }).ToList();

            sale.Payments = request.Payments.Select(p => new Payment
            {
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                ReferenceNo = p.ReferenceNo
            }).ToList();

            sale.TotalAmount = sale.SaleDetails.Sum(x => x.Total);

            _saleRepo.Update(sale);
            await _saleRepo.SaveChangesAsync();

            return await GetByIdAsync(sale.SaleId);
        }

        public async Task<bool> DeleteAsync(int saleId)
        {
            var sale = await _saleRepo.GetByIdAsync(saleId);
            if (sale == null) return false;

            _saleRepo.Delete(sale);
            await _saleRepo.SaveChangesAsync();
            return true;
        }
    }
}
