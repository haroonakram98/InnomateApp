using MediatR;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Attributes;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetActiveSuppliers
{
    [NoTransaction]
    public class GetActiveSuppliersQuery : IRequest<List<SupplierResponse>>, IBaseQuery
    {
    }
}
