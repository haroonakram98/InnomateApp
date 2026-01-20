using MediatR;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Requests;
using InnomateApp.Application.DTOs.Suppliers.Responses;

namespace InnomateApp.Application.Features.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommand : IRequest<Result<SupplierResponse>>
    {
        public CreateSupplierRequest Request { get; }

        public CreateSupplierCommand(CreateSupplierRequest request)
        {
            Request = request;
        }
    }
}
