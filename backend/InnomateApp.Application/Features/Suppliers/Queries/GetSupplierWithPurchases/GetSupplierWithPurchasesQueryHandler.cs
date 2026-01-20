using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierWithPurchases
{
    public class GetSupplierWithPurchasesQueryHandler : IRequestHandler<GetSupplierWithPurchasesQuery, Result<SupplierDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetSupplierWithPurchasesQueryHandler> _logger;

        public GetSupplierWithPurchasesQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetSupplierWithPurchasesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<SupplierDetailResponse>> Handle(GetSupplierWithPurchasesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            var supplier = await _unitOfWork.Suppliers.GetSupplierWithPurchasesAsync(request.SupplierId);

            if (supplier == null || supplier.TenantId != tenantId)
            {
                return Result<SupplierDetailResponse>.NotFound($"Supplier with ID {request.SupplierId} not found.");
            }

            var receivedPurchases = supplier.Purchases.Where(p => p.Status == "Received").ToList();
            
            return Result<SupplierDetailResponse>.Success(new SupplierDetailResponse
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                ContactPerson = supplier.ContactPerson,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt,
                TotalPurchases = supplier.Purchases.Count,
                TotalPurchaseAmount = receivedPurchases.Sum(p => p.TotalAmount),
                PendingPurchases = supplier.Purchases.Count(p => p.Status == "Pending"),
                LastPurchaseDate = supplier.Purchases
                    .OrderByDescending(p => p.PurchaseDate)
                    .FirstOrDefault()?.PurchaseDate
            });
        }
    }
}
