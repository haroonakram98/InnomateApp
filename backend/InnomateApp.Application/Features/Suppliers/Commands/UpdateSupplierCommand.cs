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
    public class UpdateSupplierCommand : IRequest<Result<SupplierDto>>
    {
        public int Id { get; set; }
        public UpdateSupplierDto SupplierDto { get; set; } = null!;
    }

    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.SupplierDto)
                .NotNull()
                .SetValidator(new UpdateSupplierDtoValidator());
        }
    }

    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<SupplierDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateSupplierCommandHandler> _logger;

        public UpdateSupplierCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<UpdateSupplierCommandHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating supplier ID: {SupplierId}", request.Id);

                var existingSupplier = await _uow.Suppliers.GetByIdAsync(request.Id);
                if (existingSupplier == null)
                {
                    _logger.LogWarning("Supplier update failed: ID {SupplierId} not found", request.Id);
                    return Result<SupplierDto>.NotFound($"Supplier with ID {request.Id} not found.");
                }

                // Check for duplicate name/email excluding self
                var isDuplicate = await _uow.Suppliers.SupplierExistsAsync(
                    request.Id,
                    request.SupplierDto.Name.Trim(),
                    request.SupplierDto.Email?.Trim() ?? "");

                if (isDuplicate)
                {
                    return Result<SupplierDto>.Failure("Another supplier with the same name or email already exists.");
                }

                // Use Domain Update method
                existingSupplier.Update(
                    request.SupplierDto.Name.Trim(),
                    request.SupplierDto.Email?.Trim().ToLower(),
                    request.SupplierDto.Phone?.Trim(),
                    request.SupplierDto.Address?.Trim(),
                    request.SupplierDto.ContactPerson?.Trim(),
                    request.SupplierDto.Notes?.Trim()
                );

                await _uow.Suppliers.UpdateAsync(existingSupplier);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Supplier updated successfully: {SupplierId}", request.Id);

                return Result<SupplierDto>.Success(_mapper.Map<SupplierDto>(existingSupplier));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier {SupplierId}", request.Id);
                return Result<SupplierDto>.Failure("An unexpected error occurred while updating the supplier.");
            }
        }
    }
}
