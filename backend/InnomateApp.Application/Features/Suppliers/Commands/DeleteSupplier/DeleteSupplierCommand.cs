using MediatR;
using InnomateApp.Application.Common;

namespace InnomateApp.Application.Features.Suppliers.Commands.DeleteSupplier
{
    public class DeleteSupplierCommand : IRequest<Result<bool>>
    {
        public int SupplierId { get; }

        public DeleteSupplierCommand(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
}
