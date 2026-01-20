using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<ProductResponse>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var tenantId = _tenantProvider.GetTenantId();

            var product = await _unitOfWork.Products.GetProductWithStockInfoAsync(request.ProductId);
            
            if (product == null || product.TenantId != tenantId)
            {
                return Result<ProductResponse>.NotFound($"Product with ID {request.ProductId} not found");
            }
            
            // Validate Category exists if changed
            if (product.CategoryId != request.CategoryId)
            {
                 var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                 if (category == null || category.TenantId != tenantId)
                 {
                     return Result<ProductResponse>.Failure($"Category with ID {request.CategoryId} does not exist.");
                 }
            }

            // Check SKU uniqueness if changed
            if (product.SKU != request.SKU && !string.IsNullOrEmpty(request.SKU))
            {
                var skuExists = await _unitOfWork.Products.ProductSkuExistsAsync(request.SKU);
                if (skuExists)
                {
                    return Result<ProductResponse>.Failure($"Product with SKU '{request.SKU}' already exists.");
                }
            }

            // Update Product Domain Entity
            product.Update(
                request.Name,
                request.CategoryId,
                request.SKU,
                request.DefaultSalePrice,
                request.ReorderLevel
            );

            if (request.IsActive && !product.IsActive) product.Activate();
            if (!request.IsActive && product.IsActive) product.Deactivate();

            await _unitOfWork.Products.UpdateAsync(product);
            
            // Ensure we save changes to persist updates
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product '{ProductName}' updated (ID: {ProductId})", product.Name, product.ProductId);
            
            // Map Response
            var currentStock = product.StockSummary?.Balance ?? 0;
            
            return Result<ProductResponse>.Success(new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "", // Category might be null if not included, but repo includes it
                SKU = product.SKU,
                ReorderLevel = product.ReorderLevel,
                IsActive = product.IsActive,
                DefaultSalePrice = product.DefaultSalePrice,
                CurrentStock = currentStock,
                StockStatus = product.IsLowStock(currentStock) ? "Low Stock" : "In Stock"
            });
        }
    }
}
