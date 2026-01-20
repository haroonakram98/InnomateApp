using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierStats
{
    public class GetSupplierStatsQueryHandler : IRequestHandler<GetSupplierStatsQuery, Result<SupplierStatsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetSupplierStatsQueryHandler> _logger;

        public GetSupplierStatsQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetSupplierStatsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<SupplierStatsResponse>> Handle(GetSupplierStatsQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            var supplier = await _unitOfWork.Suppliers.GetSupplierWithPurchasesAsync(request.SupplierId);

            if (supplier == null || supplier.TenantId != tenantId)
            {
                return Result<SupplierStatsResponse>.NotFound($"Supplier with ID {request.SupplierId} not found.");
            }

            var receivedPurchases = supplier.Purchases.Where(p => p.Status == "Received").ToList();
            var totalPurchases = supplier.Purchases.Count;
            var totalAmount = receivedPurchases.Sum(p => p.TotalAmount);
            var completedPurchases = receivedPurchases.Count;

            var stats = new SupplierStatsResponse
            {
                TotalPurchases = totalPurchases,
                TotalPurchaseAmount = totalAmount,
                PendingPurchases = supplier.Purchases.Count(p => p.Status == "Pending"),
                LastPurchaseDate = supplier.Purchases
                    .OrderByDescending(p => p.PurchaseDate)
                    .FirstOrDefault()?.PurchaseDate,
                AveragePurchaseAmount = completedPurchases > 0 ? totalAmount / completedPurchases : 0,
                CompletedPurchases = completedPurchases
            };

            return Result<SupplierStatsResponse>.Success(stats);
        }
    }
}
