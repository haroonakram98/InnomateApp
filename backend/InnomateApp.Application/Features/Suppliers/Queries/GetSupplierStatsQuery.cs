using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.IServices;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetSupplierStatsQuery : IRequest<Result<SupplierStatsResponse>>
    {
        public int Id { get; set; }
    }

    public class GetSupplierStatsQueryHandler : IRequestHandler<GetSupplierStatsQuery, Result<SupplierStatsResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetSupplierStatsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<SupplierStatsResponse>> Handle(GetSupplierStatsQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _uow.Suppliers.GetSupplierWithPurchasesAsync(request.Id);
            if (supplier == null)
                return Result<SupplierStatsResponse>.NotFound("Supplier not found.");

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
