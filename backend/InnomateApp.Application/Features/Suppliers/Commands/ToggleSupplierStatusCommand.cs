using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Commands
{
    public class ToggleSupplierStatusCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class ToggleSupplierStatusCommandHandler : IRequestHandler<ToggleSupplierStatusCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ToggleSupplierStatusCommandHandler> _logger;

        public ToggleSupplierStatusCommandHandler(IUnitOfWork uow, ILogger<ToggleSupplierStatusCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ToggleSupplierStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var supplier = await _uow.Suppliers.GetByIdAsync(request.Id);
                if (supplier == null)
                    return Result<bool>.NotFound($"Supplier ID {request.Id} not found.");

                supplier.ToggleStatus();
                await _uow.Suppliers.UpdateAsync(supplier);
                await _uow.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(supplier.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for supplier {Id}", request.Id);
                return Result<bool>.Failure("An error occurred.");
            }
        }
    }
}
