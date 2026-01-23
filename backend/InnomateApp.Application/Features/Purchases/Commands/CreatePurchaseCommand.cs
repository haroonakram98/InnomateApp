using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Commands
{
    public class CreatePurchaseCommand : IRequest<Result<PurchaseResponseDto>>
    {
        public CreatePurchaseDto CreateDto { get; set; } = null!;
    }

    public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Result<PurchaseResponseDto>>
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IMapper _mapper;

        public CreatePurchaseCommandHandler(IPurchaseService purchaseService, IMapper mapper)
        {
            _purchaseService = purchaseService;
            _mapper = mapper;
        }

        public async Task<Result<PurchaseResponseDto>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var purchase = new Purchase
            {
                SupplierId = request.CreateDto.SupplierId,
                PurchaseDate = request.CreateDto.PurchaseDate,
                InvoiceNo = request.CreateDto.InvoiceNo,
                Notes = request.CreateDto.Notes,
                CreatedBy = request.CreateDto.CreatedBy,
                PurchaseDetails = request.CreateDto.PurchaseDetails.Select(detail => new PurchaseDetail
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost,
                    ExpiryDate = detail.ExpiryDate,
                    BatchNo = detail.BatchNo
                }).ToList()
            };

            var result = await _purchaseService.CreatePurchaseAsync(purchase);
            var responseDto = _mapper.Map<PurchaseResponseDto>(result);

            return Result<PurchaseResponseDto>.Success(responseDto);
        }
    }
}
