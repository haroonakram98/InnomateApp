using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Customers.Commands
{
    public class ToggleCustomerStatusCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }

        public ToggleCustomerStatusCommand(int id)
        {
            Id = id;
        }
    }

    public class ToggleCustomerStatusCommandHandler : IRequestHandler<ToggleCustomerStatusCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ToggleCustomerStatusCommandHandler> _logger;

        public ToggleCustomerStatusCommandHandler(IUnitOfWork uow, ILogger<ToggleCustomerStatusCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ToggleCustomerStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _uow.Customers.GetByIdAsync(request.Id);
                if (customer == null)
                    return Result<bool>.Failure("Customer not found.");

                customer.ToggleStatus();
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Customer {CustomerId} status toggled to {IsActive}", 
                    customer.CustomerId, customer.IsActive);

                return Result<bool>.Success(customer.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling customer status {CustomerId}", request.Id);
                return Result<bool>.Failure("An unexpected error occurred.");
            }
        }
    }
}
