using MediatR;
using InnomateApp.Application.Common;

namespace InnomateApp.Application.Features.Suppliers.Commands.ToggleSupplierStatus
{
    public class ToggleSupplierStatusCommand : IRequest<Result<bool>>
    {
        public int SupplierId { get; }

        public ToggleSupplierStatusCommand(int supplierId)
        {
            SupplierId = supplierId;
        }
    }
}
