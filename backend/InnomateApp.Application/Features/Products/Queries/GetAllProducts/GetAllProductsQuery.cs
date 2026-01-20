using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Products.Queries.GetAllProducts
{
    [NoTransaction]
    public class GetAllProductsQuery : IRequest<List<ProductResponse>>, IBaseQuery
    {
    }
}
