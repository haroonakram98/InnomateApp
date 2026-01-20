using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<CreateSupplierCommandHandler> _logger;

        public CreateSupplierCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<CreateSupplierCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<SupplierResponse>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            var req = request.Request;
            var tenantId = _tenantProvider.GetTenantId();

            // Duplicate check
            if (await _unitOfWork.Suppliers.SupplierExistsAsync(req.Name.Trim(), req.Email?.Trim()))
            {
                return Result<SupplierResponse>.Failure("A supplier with the same name or email already exists.");
            }

            // Create entity using factory method
            var supplier = Supplier.Create(
                tenantId,
                req.Name,
                req.Email,
                req.Phone,
                req.Address
            );
            
            // Additional fields not in Factory method (Create method is simple)
            // Or we update them after creation or update Factory method.
            // Existing factory: Name, Email, Phone, Address.
            // Missing: ContactPerson, Notes.
            // I'll set them manually or add Update call.
            supplier.ContactPerson = req.ContactPerson?.Trim();
            supplier.Notes = req.Notes?.Trim();

            if (!supplier.IsValid())
            {
                return Result<SupplierResponse>.Failure("Supplier data is invalid.");
            }

            await _unitOfWork.Suppliers.AddAsync(supplier);
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
                CreatedAt = supplier.CreatedAt
            });
        }
    }
}
