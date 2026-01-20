using InnomateApp.Application.DTOs.Products.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetAllProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<List<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            // Repository handles tenant filtering via global query filter
            var products = await _unitOfWork.Products.GetAllProductsWithCategoryAsync();

            return products.Select(p => {
                var currentStock = p.StockSummary?.Balance ?? 0;
                return new ProductResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    SKU = p.SKU,
                    ReorderLevel = p.ReorderLevel,
                    IsActive = p.IsActive,
                    DefaultSalePrice = p.DefaultSalePrice,
                    CurrentStock = currentStock,
                    StockStatus = p.IsLowStock(currentStock) ? "Low Stock" : "In Stock",
                    TotalValue = p.StockSummary?.TotalValue ?? 0
                };
            }).ToList();
        }
    }
}
