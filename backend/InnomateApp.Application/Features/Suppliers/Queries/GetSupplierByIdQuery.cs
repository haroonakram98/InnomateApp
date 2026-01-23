using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Queries
{
    public class GetSupplierByIdQuery : IRequest<Result<SupplierDto>>
    {
        public int Id { get; set; }
    }

    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(request.Id);
            if (supplier == null)
                return Result<SupplierDto>.NotFound("Supplier not found.");

            return Result<SupplierDto>.Success(_mapper.Map<SupplierDto>(supplier));
        }
    }
}
