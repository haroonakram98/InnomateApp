using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Commands.ToggleSupplierStatus
{
    public class ToggleSupplierStatusCommandHandler : IRequestHandler<ToggleSupplierStatusCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<ToggleSupplierStatusCommandHandler> _logger;

        public ToggleSupplierStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<ToggleSupplierStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ToggleSupplierStatusCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId);

            if (supplier == null || supplier.TenantId != tenantId)
            {
                return Result<bool>.NotFound($"Supplier with ID {request.SupplierId} not found.");
            }

            supplier.ToggleStatus();
            await _unitOfWork.Suppliers.UpdateAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(supplier.IsActive);
        }
    }
}
