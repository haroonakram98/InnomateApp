using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Products.Queries.GetProductLookup
{
    [NoTransaction]
    public class GetProductLookupQuery : IRequest<List<ProductLookupResponse>>, IBaseQuery
    {
    }
}
