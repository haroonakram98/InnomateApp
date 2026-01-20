using InnomateApp.Application.Common;
using MediatR;

namespace InnomateApp.Application.Features.Products.Commands.DeleteProduct
{
    /// <summary>
    /// Command to delete (deactivate) a product
    /// </summary>
    public class DeleteProductCommand : IRequest<Result<bool>>
    {
        public int ProductId { get; }

        public DeleteProductCommand(int productId)
        {
            ProductId = productId;
        }
    }
}
