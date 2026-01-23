using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public int ProductId { get; set; }

        public GetProductByIdQuery(int productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductService _productService;

        public GetProductByIdQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(request.ProductId);
            if (product == null)
                return Result<ProductDto>.NotFound("Product not found");

            return Result<ProductDto>.Success(product);
        }
    }
}
