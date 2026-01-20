using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetAllSuppliers
{
    public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, List<SupplierResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<GetAllSuppliersQueryHandler> _logger;

        public GetAllSuppliersQueryHandler(
            IUnitOfWork unitOfWork,
            ITenantProvider tenantProvider,
            ILogger<GetAllSuppliersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<List<SupplierResponse>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            var suppliers = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? await _unitOfWork.Suppliers.GetAllAsync()
                : await _unitOfWork.Suppliers.SearchSuppliersAsync(request.SearchTerm);

            return suppliers.Select(s => new SupplierResponse
            {
                SupplierId = s.SupplierId,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                ContactPerson = s.ContactPerson,
                Notes = s.Notes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }
    }
}
