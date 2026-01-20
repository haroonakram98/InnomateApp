using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Common;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<CreateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var tenantId = _tenantProvider.GetTenantId();

            // validation: Check if SKU is unique (if provided)
            if (!string.IsNullOrEmpty(request.SKU))
            {
                var skuExists = await _unitOfWork.Products.ProductSkuExistsAsync(request.SKU);
                if (skuExists)
                {
                    return Result<ProductResponse>.Failure($"Product with SKU '{request.SKU}' already exists.");
                }
            }
            
            // Validate Category exists
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            if (category == null || category.TenantId != tenantId)
            {
                 return Result<ProductResponse>.Failure($"Category with ID {request.CategoryId} does not exist.");
            }

            // Create Product
            var product = Product.Create(
                tenantId,
                request.Name,
                request.CategoryId,
                request.SKU,
                request.DefaultSalePrice,
                request.ReorderLevel
            );

            // Set Active status (Create factory sets to true by default, but request might specify false)
            if (!request.IsActive)
            {
                product.Deactivate();
            }

            await _unitOfWork.Products.AddAsync(product);
            
            // Note: We need to save changes here to get the generated ProductId for the StockSummary?
            // Usually EF Core tracks this, but since StockSummary.Create takes productId, we might need the ID.
            // However, typically you can start with setting the navigation property. 
            // `StockSummary.Product = product` 
            // But let's look at `StockSummary.Create`: it takes `productId`.
            
            // Actually, since they are in the same transaction (UnitOfWork), 
            // we can add the product, then effectively we rely on EF Core to resolve the ID if we used navigation prop.
            // But `StockSummary` entity has `ProductId` FK. 
            
            // Best practice: Use navigation property to ensure EF resolves ID.
            var stockSummary = StockSummary.Create(tenantId, 0); // 0 temporary
            stockSummary.Product = product; // Link navigation property
            
            await _unitOfWork.Stock.AddAsync(stockSummary);

            // EnhancedTransactionBehavior calls SaveChangesAsync at the end. 
            // EF Core is smart enough to insert Product, get ID, insert StockSummary with that ID due to navigation prop.

            _logger.LogInformation("Product '{ProductName}' created", product.Name);

            // We return response. Since SaveChanges hasn't happened yet, ProductId might be 0.
            // But the Controller returns CreatedAtAction. 
            // Ideally, the ID should be populated after SaveChanges.
            // The Behavior pipeline: Handle -> SaveChanges. 
            // So we return the object, invalid ID?
            // Actually, `EnhancedTransactionBehavior` calls `await next()`, *then* `SaveChangesAsync`.
            // So the Result returned here will have ID=0.
            
            // Correction: This is a known issue with MediatR Pipeline + EF Core AutoInc IDs if we return the ID.
            // To fix this, we can either:
            // 1. Manually `SaveChangesAsync` here (inside the transaction). The behavior will call it again (no-op).
            // 2. Or assume the caller doesn't need the ID immediately (unlikely for REST API).
            
            // I will manually `SaveChangesAsync` here to ensure ID is generated for the response.
            // Since `EnhancedTransactionBehavior` wraps this in a transaction, it is safe.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProductResponse>.Success(new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                SKU = product.SKU,
                ReorderLevel = product.ReorderLevel,
                IsActive = product.IsActive,
                DefaultSalePrice = product.DefaultSalePrice,
                CurrentStock = 0,
                StockStatus = "Unknown" // Initial state
            });
        }
    }
}
