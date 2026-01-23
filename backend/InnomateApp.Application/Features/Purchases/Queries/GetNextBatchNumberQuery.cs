using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Queries
{
    public class GetNextBatchNumberQuery : IRequest<Result<string>>
    {
    }

    public class GetNextBatchNumberQueryHandler : IRequestHandler<GetNextBatchNumberQuery, Result<string>>
    {
        private readonly IPurchaseService _purchaseService;

        public GetNextBatchNumberQueryHandler(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        public async Task<Result<string>> Handle(GetNextBatchNumberQuery request, CancellationToken cancellationToken)
        {
            var nextNo = await _purchaseService.GetNextBatchNumberAsync();
            return Result<string>.Success(nextNo);
        }
    }
}
