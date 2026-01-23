using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Queries
{
    public class GetPurchaseByIdQuery : IRequest<Result<PurchaseResponseDto>>
    {
        public int PurchaseId { get; set; }

        public GetPurchaseByIdQuery(int purchaseId)
        {
            PurchaseId = purchaseId;
        }
    }

    public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, Result<PurchaseResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPurchaseByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<PurchaseResponseDto>> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
        {
            var purchase = await _uow.Purchases.GetPurchaseWithDetailsAsync(request.PurchaseId);
            if (purchase == null)
                return Result<PurchaseResponseDto>.NotFound("Purchase not found");

            var dto = _mapper.Map<PurchaseResponseDto>(purchase);
            return Result<PurchaseResponseDto>.Success(dto);
        }
    }
}
