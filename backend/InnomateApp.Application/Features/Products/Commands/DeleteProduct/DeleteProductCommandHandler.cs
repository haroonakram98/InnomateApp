using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId);

            if (product == null || product.TenantId != tenantId)
            {
               return Result<bool>.NotFound($"Product with ID {command.ProductId} not found");
            }

            // Soft Delete (Deactivate)
            product.Deactivate();
            
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product '{ProductName}' deactivated (ID: {ProductId})", product.Name, product.ProductId);
            
            return Result<bool>.Success(true);
        }
    }
}
