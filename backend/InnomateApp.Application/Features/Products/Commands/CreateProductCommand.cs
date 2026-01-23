using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Result<ProductDto>>
    {
        public CreateProductDto CreateDto { get; set; } = null!;
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var result = await _productService.CreateAsync(request.CreateDto);
            return Result<ProductDto>.Success(result);
        }
    }
}
