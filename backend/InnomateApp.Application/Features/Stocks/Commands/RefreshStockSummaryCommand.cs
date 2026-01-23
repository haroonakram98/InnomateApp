using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Commands
{
    public class RefreshStockSummaryCommand : IRequest<Result<bool>>
    {
        public int ProductId { get; set; }

        public RefreshStockSummaryCommand(int productId)
        {
            ProductId = productId;
        }
    }

    public class RefreshStockSummaryCommandHandler : IRequestHandler<RefreshStockSummaryCommand, Result<bool>>
    {
        private readonly IStockService _stockService;

        public RefreshStockSummaryCommandHandler(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<Result<bool>> Handle(RefreshStockSummaryCommand request, CancellationToken cancellationToken)
        {
            var success = await _stockService.UpdateStockSummaryAsync(request.ProductId);
            return success ? Result<bool>.Success(true) : Result<bool>.Failure("Failed to refresh stock summary");
        }
    }
}
