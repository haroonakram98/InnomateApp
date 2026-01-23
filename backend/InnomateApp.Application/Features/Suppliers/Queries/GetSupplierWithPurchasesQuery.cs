using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetSupplierWithPurchasesQuery : IRequest<Result<SupplierDetailDto>>
    {
        public int Id { get; set; }
    }

    public class GetSupplierWithPurchasesQueryHandler : IRequestHandler<GetSupplierWithPurchasesQuery, Result<SupplierDetailDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSupplierWithPurchasesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<SupplierDetailDto>> Handle(GetSupplierWithPurchasesQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _uow.Suppliers.GetSupplierWithPurchasesAsync(request.Id);
            if (supplier == null)
                return Result<SupplierDetailDto>.NotFound("Supplier not found.");

            var dto = _mapper.Map<SupplierDetailDto>(supplier);
            return Result<SupplierDetailDto>.Success(dto);
        }
    }
}
