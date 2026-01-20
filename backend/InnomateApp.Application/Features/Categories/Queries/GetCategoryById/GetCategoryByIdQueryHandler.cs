using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Categories.Queries.GetCategoryById
{
    /// <summary>
    /// Handler for GetCategoryByIdQuery
    /// </summary>
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

        public GetCategoryByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetCategoryByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();

            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
            
            if (category == null || category.TenantId != tenantId)
            {
                return Result<CategoryResponse>.NotFound($"Category with ID {request.CategoryId} not found");
            }

            var productCount = await _unitOfWork.Categories.GetProductCountAsync(request.CategoryId);

            _logger.LogDebug("Retrieved category {CategoryId} for tenant {TenantId}", 
                request.CategoryId, tenantId);

            return Result<CategoryResponse>.Success(new CategoryResponse
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ProductCount = productCount
            });
        }
    }
}
