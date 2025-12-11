using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetSuppliersQuery : IRequest<Result<List<SupplierDto>>>
    {
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, Result<List<SupplierDto>>>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetSuppliersQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            var supplierDtos = _mapper.Map<List<SupplierDto>>(suppliers);

            return Result<List<SupplierDto>>.Success(supplierDtos);
        }
    }
}
