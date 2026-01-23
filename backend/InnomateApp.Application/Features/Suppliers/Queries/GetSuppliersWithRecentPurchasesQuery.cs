using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetSuppliersWithRecentPurchasesQuery : IRequest<Result<List<SupplierDto>>>
    {
        public int Days { get; set; } = 30;
    }

    public class GetSuppliersWithRecentPurchasesQueryHandler : IRequestHandler<GetSuppliersWithRecentPurchasesQuery, Result<List<SupplierDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSuppliersWithRecentPurchasesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<List<SupplierDto>>> Handle(GetSuppliersWithRecentPurchasesQuery request, CancellationToken cancellationToken)
        {
            var fromDate = DateTime.UtcNow.AddDays(-request.Days);
            var suppliers = await _uow.Suppliers.GetSuppliersWithRecentPurchasesAsync(fromDate);
            var dtos = _mapper.Map<List<SupplierDto>>(suppliers);
            return Result<List<SupplierDto>>.Success(dtos);
        }
    }
}
