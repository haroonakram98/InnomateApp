using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierLookup
{
    public class GetSupplierLookupQueryHandler : IRequestHandler<GetSupplierLookupQuery, List<SupplierLookupResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetSupplierLookupQueryHandler> _logger;

        public GetSupplierLookupQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetSupplierLookupQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<SupplierLookupResponse>> Handle(GetSupplierLookupQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Suppliers.GetLookupAsync();
            return result.ToList();
        }
    }
}
