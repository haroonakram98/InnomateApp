using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Commands
{
    public class CancelPurchaseCommand : IRequest<Result<bool>>
    {
        public int PurchaseId { get; set; }

        public CancelPurchaseCommand(int purchaseId)
        {
            PurchaseId = purchaseId;
        }
    }

    public class CancelPurchaseCommandHandler : IRequestHandler<CancelPurchaseCommand, Result<bool>>
    {
        private readonly IPurchaseService _purchaseService;

        public CancelPurchaseCommandHandler(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        public async Task<Result<bool>> Handle(CancelPurchaseCommand request, CancellationToken cancellationToken)
        {
            var success = await _purchaseService.CancelPurchaseAsync(request.PurchaseId);
            return Result<bool>.Success(success);
        }
    }
}
