using MediatR;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using InnomateApp.Application.Attributes;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetAllSuppliers
{
    [NoTransaction]
    public class GetAllSuppliersQuery : IRequest<List<SupplierResponse>>, IBaseQuery
    {
        public string SearchTerm { get; set; }

        public GetAllSuppliersQuery(string searchTerm = null)
        {
            SearchTerm = searchTerm;
        }
    }
}
