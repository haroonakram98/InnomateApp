using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Commands
{
    public class CreateSupplierCommand : IRequest<Result<int>>
    {
        public CreateSupplierDto SupplierDto { get; set; } = null!;
    }

    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<int>>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSupplierCommandHandler> _logger;

        public CreateSupplierCommandHandler(
            ISupplierRepository supplierRepository,
            IMapper mapper,
            ILogger<CreateSupplierCommandHandler> logger)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new supplier: {SupplierName}", request.SupplierDto.Name);

                // Validate the DTO
                var validator = new CreateSupplierDtoValidator();
                var validationResult = await validator.ValidateAsync(request.SupplierDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for supplier creation: {Errors}", string.Join(", ", errorMessages));
                    return Result<int>.Failure(errorMessages);
                }

                // Check for duplicate supplier
                var supplierExists = await _supplierRepository.SupplierExistsAsync(
                    request.SupplierDto.Name.Trim(),
                    request.SupplierDto.Email?.Trim() ?? "");

                if (supplierExists)
                {
                    _logger.LogWarning("Duplicate supplier found: {Name} or {Email}", request.SupplierDto.Name, request.SupplierDto.Email);
                    return Result<int>.Failure("A supplier with the same name or email already exists.");
                }

                // Map and create supplier
                var supplier = _mapper.Map<Supplier>(request.SupplierDto);

                // Ensure critical properties are set
                supplier.Name = supplier.Name.Trim();
                supplier.Email = supplier.Email?.Trim().ToLower();
                supplier.Phone = supplier.Phone?.Trim();
                supplier.IsActive = true;
                supplier.CreatedAt = DateTime.Now;

                // Additional business validation
                if (!supplier.IsValid())
                {
                    _logger.LogWarning("Supplier entity validation failed for: {SupplierName}", request.SupplierDto.Name);
                    return Result<int>.Failure("Supplier data is invalid. Please check name, email, and phone.");
                }

                var createdSupplier = await _supplierRepository.AddAsync(supplier);

                _logger.LogInformation("Supplier created successfully: {SupplierName} (ID: {SupplierId})",
                    supplier.Name, createdSupplier.SupplierId);

                return Result<int>.Success(createdSupplier.SupplierId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier {SupplierName}", request.SupplierDto.Name);
                return Result<int>.Failure("An unexpected error occurred while creating the supplier.");
            }
        }
    }
}