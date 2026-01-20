using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetActiveSuppliers
{
    public class GetActiveSuppliersQueryHandler : IRequestHandler<GetActiveSuppliersQuery, List<SupplierResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetActiveSuppliersQueryHandler> _logger;

        public GetActiveSuppliersQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetActiveSuppliersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<SupplierResponse>> Handle(GetActiveSuppliersQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _unitOfWork.Suppliers.GetActiveSuppliersAsync();
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
