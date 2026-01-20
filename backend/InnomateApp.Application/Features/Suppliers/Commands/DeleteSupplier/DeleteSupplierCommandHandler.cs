using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Commands.DeleteSupplier
{
    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<DeleteSupplierCommandHandler> _logger;

        public DeleteSupplierCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<DeleteSupplierCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId);

            if (supplier == null || supplier.TenantId != tenantId)
            {
                return Result<bool>.NotFound($"Supplier with ID {request.SupplierId} not found.");
            }

            // Check if supplier has purchases
            var purchaseCount = await _unitOfWork.Suppliers.GetSupplierPurchaseCountAsync(request.SupplierId);
            if (purchaseCount > 0)
            {
                return Result<bool>.Failure("Cannot delete supplier with existing purchases. Consider deactivating instead.");
            }

            await _unitOfWork.Suppliers.DeleteAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Supplier deleted successfully: {SupplierName} (ID: {SupplierId})", supplier.Name, supplier.SupplierId);

            return Result<bool>.Success(true);
        }
    }
}
