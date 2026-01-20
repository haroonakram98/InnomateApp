using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetProductByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            
            // Use specialized query that includes StockSummary
            var product = await _unitOfWork.Products.GetProductWithStockInfoAsync(request.ProductId);
            
            if (product == null || product.TenantId != tenantId)
            {
                return Result<ProductResponse>.NotFound($"Product with ID {request.ProductId} not found");
            }
            
            var currentStock = product.StockSummary?.Balance ?? 0;

            return Result<ProductResponse>.Success(new ProductResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                SKU = product.SKU,
                ReorderLevel = product.ReorderLevel,
                IsActive = product.IsActive,
                DefaultSalePrice = product.DefaultSalePrice,
                CurrentStock = currentStock,
                StockStatus = product.IsLowStock(currentStock) ? "Low Stock" : "In Stock",
                TotalValue = product.StockSummary?.TotalValue ?? 0
            });
        }
    }
}
