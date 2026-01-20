using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierLookup
{
    [NoTransaction]
    public class GetSupplierLookupQuery : IRequest<List<SupplierLookupResponse>>, IBaseQuery
    {
    }
}
