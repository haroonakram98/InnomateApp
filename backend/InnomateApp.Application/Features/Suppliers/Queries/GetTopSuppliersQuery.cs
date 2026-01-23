using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetTopSuppliersQuery : IRequest<Result<List<SupplierDto>>>
    {
        public int Count { get; set; } = 10;
    }

    public class GetTopSuppliersQueryHandler : IRequestHandler<GetTopSuppliersQuery, Result<List<SupplierDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetTopSuppliersQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<List<SupplierDto>>> Handle(GetTopSuppliersQuery request, CancellationToken cancellationToken)
        {
            var topSuppliers = await _uow.Suppliers.GetTopSuppliersByPurchaseAmountAsync(request.Count);
            var dtos = _mapper.Map<List<SupplierDto>>(topSuppliers);
            return Result<List<SupplierDto>>.Success(dtos);
        }
    }
}
