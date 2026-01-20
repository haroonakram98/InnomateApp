using MediatR;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs.Suppliers.Requests;
using InnomateApp.Application.DTOs.Suppliers.Responses;

namespace InnomateApp.Application.Features.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommand : IRequest<Result<SupplierResponse>>
    {
        public UpdateSupplierRequest Request { get; }

        public UpdateSupplierCommand(UpdateSupplierRequest request)
        {
            Request = request;
        }
    }
}
