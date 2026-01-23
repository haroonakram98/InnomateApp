using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<Result<IEnumerable<ProductDto>>>
    {
        public string? Search { get; set; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IEnumerable<ProductDto>>>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<IEnumerable<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                products = products.Where(p => 
                    p.Name.ToLower().Contains(search) || 
                    (p.SKU != null && p.SKU.ToLower().Contains(search))
                );
            }

            return Result<IEnumerable<ProductDto>>.Success(products);
        }
    }
}
