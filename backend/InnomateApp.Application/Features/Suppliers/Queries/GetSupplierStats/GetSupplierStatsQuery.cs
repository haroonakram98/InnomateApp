using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Suppliers.Queries.GetSupplierStats
{
    [NoTransaction]
    public class GetSupplierStatsQuery : IRequest<Result<SupplierStatsResponse>>, IBaseQuery
    {
        public int SupplierId { get; }

        public GetSupplierStatsQuery(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
}
