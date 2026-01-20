using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Categories.Queries.GetAllCategories
{
    /// <summary>
    /// Handler for GetAllCategoriesQuery
    /// </summary>
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<List<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            
            var categories = await _unitOfWork.Categories.GetAllWithProductCountAsync(tenantId);

            _logger.LogDebug("Retrieved {Count} categories for tenant {TenantId}", 
                categories.Count, tenantId);

            return categories;
        }
    }
}
