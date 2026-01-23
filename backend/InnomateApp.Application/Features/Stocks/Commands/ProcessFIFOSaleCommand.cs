using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Commands
{
    public class ProcessFIFOSaleCommand : IRequest<Result<FIFOSaleResultDto>>
    {
        public FIFOSaleRequestDto Request { get; set; } = null!;
    }

    public class ProcessFIFOSaleCommandHandler : IRequestHandler<ProcessFIFOSaleCommand, Result<FIFOSaleResultDto>>
    {
        private readonly IStockService _stockService;

        public ProcessFIFOSaleCommandHandler(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<Result<FIFOSaleResultDto>> Handle(ProcessFIFOSaleCommand command, CancellationToken cancellationToken)
        {
            var result = await _stockService.ProcessFIFOSaleAsync(
                command.Request.ProductId,
                command.Request.Quantity,
                command.Request.SaleReferenceId
            );

            return result.Success ? Result<FIFOSaleResultDto>.Success(result) : Result<FIFOSaleResultDto>.Failure(result.Message);
        }
    }
}
