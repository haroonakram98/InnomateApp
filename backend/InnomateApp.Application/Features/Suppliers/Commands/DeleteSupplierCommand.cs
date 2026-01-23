using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Commands
{
    public class DeleteSupplierCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteSupplierCommandHandler> _logger;

        public DeleteSupplierCommandHandler(IUnitOfWork uow, ILogger<DeleteSupplierCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting supplier ID: {SupplierId}", request.Id);

                var supplier = await _uow.Suppliers.GetSupplierWithPurchasesAsync(request.Id);
                if (supplier == null)
                    return Result<bool>.NotFound($"Supplier with ID {request.Id} not found.");

                // Check business rule: Cannot delete supplier with purchases
                if (supplier.Purchases.Any())
                {
                    _logger.LogWarning("Cannot delete supplier {Id} because they have purchases.", request.Id);
                    return Result<bool>.Failure("Cannot delete supplier with existing purchases. Deactivate them instead.");
                }

                await _uow.Suppliers.DeleteAsync(supplier);
                await _uow.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier {SupplierId}", request.Id);
                return Result<bool>.Failure("An error occurred while deleting the supplier.");
            }
        }
    }
}
