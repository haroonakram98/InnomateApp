using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Commands
{
    public class ReceivePurchaseCommand : IRequest<Result<PurchaseResponseDto>>
    {
        public int PurchaseId { get; set; }

        public ReceivePurchaseCommand(int purchaseId)
        {
            PurchaseId = purchaseId;
        }
    }

    public class ReceivePurchaseCommandHandler : IRequestHandler<ReceivePurchaseCommand, Result<PurchaseResponseDto>>
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IMapper _mapper;

        public ReceivePurchaseCommandHandler(IPurchaseService purchaseService, IMapper mapper)
        {
            _purchaseService = purchaseService;
            _mapper = mapper;
        }

        public async Task<Result<PurchaseResponseDto>> Handle(ReceivePurchaseCommand request, CancellationToken cancellationToken)
        {
            var result = await _purchaseService.ReceivePurchaseAsync(request.PurchaseId);
            var responseDto = _mapper.Map<PurchaseResponseDto>(result);

            return Result<PurchaseResponseDto>.Success(responseDto);
        }
    }
}
