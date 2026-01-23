using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Customers.Commands
{
    public class DeleteCustomerCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }

        public DeleteCustomerCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteCustomerCommandHandler> _logger;

        public DeleteCustomerCommandHandler(IUnitOfWork uow, ILogger<DeleteCustomerCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _uow.Customers.GetByIdAsync(request.Id);
                if (customer == null)
                    return Result<bool>.Failure("Customer not found.");

                _logger.LogInformation("Deleting customer: {CustomerName} (ID: {CustomerId})", customer.Name, customer.CustomerId);

                // Check for dependencies before deletion (optional)
                // if (await _uow.Sales.AnyAsync(s => s.CustomerId == request.Id))
                //    return Result<bool>.Failure("Cannot delete customer with existing sales records.");

                await _uow.Customers.DeleteAsync(customer);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Customer deleted successfully: {CustomerId}", request.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", request.Id);
                return Result<bool>.Failure("An unexpected error occurred while deleting the customer.");
            }
        }
    }
}
