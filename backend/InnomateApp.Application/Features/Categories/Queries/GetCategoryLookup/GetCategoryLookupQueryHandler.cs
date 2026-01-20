using InnomateApp.Application.DTOs.Categories.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Categories.Queries.GetCategoryLookup
{
    /// <summary>
    /// Handler for GetCategoryLookupQuery
    /// Returns lightweight category list for dropdowns
    /// </summary>
    public class GetCategoryLookupQueryHandler : IRequestHandler<GetCategoryLookupQuery, List<CategoryLookupResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetCategoryLookupQueryHandler> _logger;

        public GetCategoryLookupQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetCategoryLookupQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<List<CategoryLookupResponse>> Handle(GetCategoryLookupQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _tenantProvider.GetTenantId();
            
            var categories = await _unitOfWork.Categories.GetLookupAsync(tenantId);

            _logger.LogDebug("Retrieved {Count} categories for lookup (tenant {TenantId})", 
                categories.Count, tenantId);

            return categories;
        }
    }
}
