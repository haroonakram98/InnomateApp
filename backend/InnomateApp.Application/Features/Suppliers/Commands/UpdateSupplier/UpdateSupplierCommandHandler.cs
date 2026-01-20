using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<SupplierResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<UpdateSupplierCommandHandler> _logger;

        public UpdateSupplierCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<UpdateSupplierCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<SupplierResponse>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var tenantId = _tenantProvider.GetTenantId();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(req.SupplierId);

            if (supplier == null || supplier.TenantId != tenantId)
            {
                return Result<SupplierResponse>.NotFound($"Supplier with ID {req.SupplierId} not found.");
            }

            // check duplicate
            if (await _unitOfWork.Suppliers.SupplierExistsAsync(req.SupplierId, req.Name.Trim(), req.Email?.Trim()))
            {
                return Result<SupplierResponse>.Failure("Another supplier with the same name or email already exists.");
            }

            supplier.Update(
                req.Name.Trim(),
                req.Email?.Trim(),
                req.Phone?.Trim(),
                req.Address?.Trim(),
                req.ContactPerson?.Trim(),
                req.Notes?.Trim()
            );

            if (!supplier.IsValid())
            {
                return Result<SupplierResponse>.Failure("Supplier data is invalid.");
            }

            await _unitOfWork.Suppliers.UpdateAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

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
