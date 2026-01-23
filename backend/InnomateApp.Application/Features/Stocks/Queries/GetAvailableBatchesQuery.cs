using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Queries
{
    public class GetAvailableBatchesQuery : IRequest<Result<IEnumerable<FifoBatchDto>>>
    {
        public int ProductId { get; set; }

        public GetAvailableBatchesQuery(int productId)
        {
            ProductId = productId;
        }
    }

    public class GetAvailableBatchesQueryHandler : IRequestHandler<GetAvailableBatchesQuery, Result<IEnumerable<FifoBatchDto>>>
    {
        private readonly IUnitOfWork _uow;

        public GetAvailableBatchesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<IEnumerable<FifoBatchDto>>> Handle(GetAvailableBatchesQuery request, CancellationToken cancellationToken)
        {
            var batches = await _uow.Stock.GetAvailableBatchesForProductAsync(request.ProductId);
            
            var dtos = batches.Select(b => new FifoBatchDto
            {
                PurchaseDetailId = b.PurchaseDetailId,
                BatchNo = b.BatchNo ?? $"BATCH-{b.PurchaseDetailId}",
                PurchaseDate = b.Purchase?.PurchaseDate ?? DateTime.Now,
                AvailableQuantity = b.RemainingQty,
                UnitCost = b.UnitCost,
                ExpiryDate = b.ExpiryDate
            });

            return Result<IEnumerable<FifoBatchDto>>.Success(dtos);
        }
    }
}
