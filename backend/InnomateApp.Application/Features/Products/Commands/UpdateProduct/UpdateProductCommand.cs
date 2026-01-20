using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Requests;
using InnomateApp.Application.DTOs.Products.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<Result<ProductResponse>>
    {
        public UpdateProductRequest Request { get; }

        public UpdateProductCommand(UpdateProductRequest request)
        {
            Request = request;
        }
    }
}
