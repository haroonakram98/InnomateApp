using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Products.Requests;
using InnomateApp.Application.DTOs.Products.Responses;
using MediatR;

namespace InnomateApp.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Result<ProductResponse>>
    {
        public CreateProductRequest Request { get; }

        public CreateProductCommand(CreateProductRequest request)
        {
            Request = request;
        }
    }
}
