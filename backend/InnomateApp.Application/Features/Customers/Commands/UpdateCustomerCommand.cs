using AutoMapper;
using FluentValidation;
using InnomateApp.Application.Common;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommand : IRequest<Result<CustomerDto>>
    {
        public int Id { get; set; }
        public UpdateCustomerDto CustomerDto { get; set; } = null!;
    }

    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.CustomerDto)
                .NotNull()
                .SetValidator(new UpdateCustomerDtoValidator());
        }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;

        public UpdateCustomerCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<UpdateCustomerCommandHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _uow.Customers.GetByIdAsync(request.Id);
                if (customer == null)
                    return Result<CustomerDto>.Failure("Customer not found.");

                _logger.LogInformation("Updating customer: {CustomerName} (ID: {CustomerId})", customer.Name, customer.CustomerId);

                customer.Update(
                    request.CustomerDto.Name,
                    request.CustomerDto.Phone,
                    request.CustomerDto.Email,
                    request.CustomerDto.Address
                );

                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Customer updated successfully: {CustomerId}", customer.CustomerId);

                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Result<CustomerDto>.Success(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", request.Id);
                return Result<CustomerDto>.Failure("An unexpected error occurred while updating the customer.");
            }
        }
    }
}
