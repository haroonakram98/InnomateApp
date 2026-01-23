using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Queries
{
    public class GetNextPurchaseNumberQuery : IRequest<Result<string>>
    {
    }

    public class GetNextPurchaseNumberQueryHandler : IRequestHandler<GetNextPurchaseNumberQuery, Result<string>>
    {
        private readonly IPurchaseService _purchaseService;

        public GetNextPurchaseNumberQueryHandler(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        public async Task<Result<string>> Handle(GetNextPurchaseNumberQuery request, CancellationToken cancellationToken)
        {
            var nextNo = await _purchaseService.GetNextPurchaseNumberAsync();
            return Result<string>.Success(nextNo);
        }
    }
}
