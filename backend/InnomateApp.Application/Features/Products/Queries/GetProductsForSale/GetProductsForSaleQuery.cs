using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Products.Queries.GetProductsForSale
{
    [NoTransaction]
    public class GetProductsForSaleQuery : IRequest<List<ProductResponse>>, IBaseQuery
    {
    }
}
