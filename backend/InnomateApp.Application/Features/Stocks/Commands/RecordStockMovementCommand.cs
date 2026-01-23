using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Commands
{
    public class RecordStockMovementCommand : IRequest<Result<bool>>
    {
        public StockMovementDto Movement { get; set; } = null!;
    }

    public class RecordStockMovementCommandHandler : IRequestHandler<RecordStockMovementCommand, Result<bool>>
    {
        private readonly IStockService _stockService;

        public RecordStockMovementCommandHandler(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<Result<bool>> Handle(RecordStockMovementCommand command, CancellationToken cancellationToken)
        {
            var success = await _stockService.RecordStockMovementAsync(command.Movement);
            return success ? Result<bool>.Success(true) : Result<bool>.Failure("Failed to record stock movement");
        }
    }
}
