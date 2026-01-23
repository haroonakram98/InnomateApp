using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<Result<ProductDto>>
    {
        public UpdateProductDto UpdateDto { get; set; } = null!;
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.UpdateAsync(request.UpdateDto);
                return Result<ProductDto>.Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                return Result<ProductDto>.NotFound(ex.Message);
            }
        }
    }
}
