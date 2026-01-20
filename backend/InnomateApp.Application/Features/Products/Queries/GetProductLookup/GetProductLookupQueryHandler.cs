using InnomateApp.Application.DTOs.Products.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Products.Queries.GetProductLookup
{
    public class GetProductLookupQueryHandler : IRequestHandler<GetProductLookupQuery, List<ProductLookupResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetProductLookupQueryHandler> _logger;

        public GetProductLookupQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetProductLookupQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<List<ProductLookupResponse>> Handle(GetProductLookupQuery request, CancellationToken cancellationToken)
        {
            // Repository internally filters by Tenant using Global Query Filter
            var products = await _unitOfWork.Products.GetLookupAsync();
            return products.ToList();
        }
    }
}
