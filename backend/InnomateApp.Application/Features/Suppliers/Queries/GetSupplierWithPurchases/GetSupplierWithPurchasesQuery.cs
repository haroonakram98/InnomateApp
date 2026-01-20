using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierWithPurchases
{
    [NoTransaction]
    public class GetSupplierWithPurchasesQuery : IRequest<Result<SupplierDetailResponse>>, IBaseQuery
    {
        public int SupplierId { get; }

        public GetSupplierWithPurchasesQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
}
