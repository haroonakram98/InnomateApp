using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Products.Queries
{
    public class GetProductsForSaleQuery : IRequest<Result<IEnumerable<ProductStockDto>>>
    {
    }

    public class GetProductsForSaleQueryHandler : IRequestHandler<GetProductsForSaleQuery, Result<IEnumerable<ProductStockDto>>>
    {
        private readonly IProductService _productService;

        public GetProductsForSaleQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<IEnumerable<ProductStockDto>>> Handle(GetProductsForSaleQuery request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetProductsForSale();
            return Result<IEnumerable<ProductStockDto>>.Success(products);
        }
    }
}
