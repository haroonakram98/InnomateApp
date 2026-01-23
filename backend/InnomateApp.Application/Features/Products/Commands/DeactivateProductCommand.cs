using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Products.Commands
{
    public class DeactivateProductCommand : IRequest<Result<bool>>
    {
        public int ProductId { get; set; }

        public DeactivateProductCommand(int productId)
        {
            ProductId = productId;
        }
    }

    public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, Result<bool>>
    {
        private readonly IProductService _productService;

        public DeactivateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<Result<bool>> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _productService.DeactivateAsync(request.ProductId);
                return Result<bool>.Success(true);
            }
            catch (KeyNotFoundException ex)
            {
                return Result<bool>.NotFound(ex.Message);
            }
        }
    }
}
