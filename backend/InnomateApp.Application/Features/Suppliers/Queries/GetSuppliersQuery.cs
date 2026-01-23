using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetSuppliersQuery : IRequest<Result<List<SupplierDto>>>
    {
        public bool ActiveOnly { get; set; } = false;
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, Result<List<SupplierDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSuppliersQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<List<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
        {
            IReadOnlyList<InnomateApp.Domain.Entities.Supplier> suppliers;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                suppliers = await _uow.Suppliers.SearchSuppliersAsync(request.SearchTerm);
            }
            else if (request.ActiveOnly)
            {
                suppliers = await _uow.Suppliers.GetActiveSuppliersAsync();
            }
            else
            {
                suppliers = await _uow.Suppliers.GetAllAsync();
            }

            var dtos = _mapper.Map<List<SupplierDto>>(suppliers);
            return Result<List<SupplierDto>>.Success(dtos);
        }
    }
}
