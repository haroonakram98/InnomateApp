using InnomateApp.Application.Attributes;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Products.Queries.GetProductById
{
    [NoTransaction]
    public class GetProductByIdQuery : IRequest<Result<ProductResponse>>, IBaseQuery
    {
        public int ProductId { get; }

        public GetProductByIdQuery(int productId)
        {
            ProductId = productId;
        }
    }
}
