using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierById
{
    [NoTransaction]
    public class GetSupplierByIdQuery : IRequest<Result<SupplierResponse>>, IBaseQuery
    {
        public int SupplierId { get; }

        public GetSupplierByIdQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
}
