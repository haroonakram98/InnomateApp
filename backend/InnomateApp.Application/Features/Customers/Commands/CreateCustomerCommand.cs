using AutoMapper;
using FluentValidation;
using InnomateApp.Application.Common;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<Result<CustomerDto>>
    {
        public CreateCustomerDto CustomerDto { get; set; } = null!;
    }

    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerDto)
                .NotNull()
                .SetValidator(new CreateCustomerDtoValidator());
        }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITenantProvider _tenantProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(
            IUnitOfWork uow,
            ITenantProvider tenantProvider,
            IMapper mapper,
            ILogger<CreateCustomerCommandHandler> _logger)
        {
            _uow = uow;
            _tenantProvider = tenantProvider;
            _mapper = mapper;
            this._logger = _logger;
        }

        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new customer: {CustomerName}", request.CustomerDto.Name);

                // Check for duplicate customer (optional business rule)
                var customerExists = await _uow.Customers.ExistsAsync(
                    request.CustomerDto.Name.Trim(),
                    request.CustomerDto.Phone?.Trim() ?? "");

                if (customerExists)
                {
                    _logger.LogWarning("Duplicate customer found: {Name}", request.CustomerDto.Name);
                    return Result<CustomerDto>.Failure("A customer with the same name or phone already exists.");
                }

                var tenantId = _tenantProvider.GetTenantId();
                var customer = Customer.Create(
                    tenantId,
                    request.CustomerDto.Name,
                    request.CustomerDto.Phone,
                    request.CustomerDto.Email,
                    request.CustomerDto.Address
                );

                await _uow.Customers.AddAsync(customer);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Customer created successfully: {CustomerName} (ID: {CustomerId})",
                    customer.Name, customer.CustomerId);

                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Result<CustomerDto>.Success(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer {CustomerName}", request.CustomerDto.Name);
                return Result<CustomerDto>.Failure("An unexpected error occurred while creating the customer.");
            }
        }
    }
}
