using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierById
{
    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetSupplierByIdQueryHandler> _logger;

        public GetSupplierByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetSupplierByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<SupplierResponse>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId);

            if (supplier == null || supplier.TenantId != tenantId)
            {
                return Result<SupplierResponse>.NotFound($"Supplier with ID {request.SupplierId} not found.");
            }

            return Result<SupplierResponse>.Success(new SupplierResponse
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                ContactPerson = supplier.ContactPerson,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            });
        }
    }
}
