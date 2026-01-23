using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using InnomateApp.Application.Common.Validators;

namespace InnomateApp.Application.Features.Suppliers.Commands
{
    public class CreateSupplierCommand : IRequest<Result<SupplierDto>>
    {
        public CreateSupplierDto SupplierDto { get; set; } = null!;
    }

    public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            RuleFor(x => x.SupplierDto)
                .NotNull()
                .SetValidator(new CreateSupplierDtoValidator());
        }
    }

    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITenantProvider _tenantProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSupplierCommandHandler> _logger;

        public CreateSupplierCommandHandler(
            IUnitOfWork uow,
            ITenantProvider tenantProvider,
            IMapper mapper,
            ILogger<CreateSupplierCommandHandler> logger)
        {
            _uow = uow;
            _tenantProvider = tenantProvider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new supplier: {SupplierName}", request.SupplierDto.Name);

                // Check for duplicate supplier
                var supplierExists = await _uow.Suppliers.SupplierExistsAsync(
                    request.SupplierDto.Name.Trim(),
                    request.SupplierDto.Email?.Trim() ?? "");

                if (supplierExists)
                {
                    _logger.LogWarning("Duplicate supplier found: {Name} or {Email}", request.SupplierDto.Name, request.SupplierDto.Email);
                    return Result<SupplierDto>.Failure("A supplier with the same name or email already exists.");
                }

                // Use Domain Factory Method (Practice 4: Entity Creation)
                var tenantId = _tenantProvider.GetTenantId();
                var supplier = Supplier.Create(
                    tenantId,
                    request.SupplierDto.Name,
                    request.SupplierDto.Email ?? string.Empty,
                    request.SupplierDto.Phone,
                    request.SupplierDto.Address
                );

                // Set additional fields not in factory
                supplier.ContactPerson = request.SupplierDto.ContactPerson?.Trim();
                supplier.Notes = request.SupplierDto.Notes?.Trim();
                
                // Practice 5: Standardize UtcNow (though already in factory, being explicit)
                supplier.CreatedAt = DateTime.UtcNow;

                // Practice: Unit of Work (Practice: SaveChanges)
                await _uow.Suppliers.AddAsync(supplier);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Supplier created successfully: {SupplierName} (ID: {SupplierId})",
                    supplier.Name, supplier.SupplierId);

                // Practice 2: AutoMapper
                var supplierDto = _mapper.Map<SupplierDto>(supplier);
                return Result<SupplierDto>.Success(supplierDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier {SupplierName}", request.SupplierDto.Name);
                return Result<SupplierDto>.Failure("An unexpected error occurred while creating the supplier.");
            }
        }
    }
}