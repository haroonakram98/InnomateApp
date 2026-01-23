using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Queries
{
    public class GetStockSummaryByIdQuery : IRequest<Result<StockSummaryDto>>
    {
        public int ProductId { get; set; }

        public GetStockSummaryByIdQuery(int productId)
        {
            ProductId = productId;
        }
    }

    public class GetStockSummaryByIdQueryHandler : IRequestHandler<GetStockSummaryByIdQuery, Result<StockSummaryDto>>
    {
        private readonly IStockService _stockService;

        public GetStockSummaryByIdQueryHandler(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<Result<StockSummaryDto>> Handle(GetStockSummaryByIdQuery request, CancellationToken cancellationToken)
        {
            var summary = await _stockService.GetStockSummaryByProductIdAsync(request.ProductId);
            return summary != null ? Result<StockSummaryDto>.Success(summary) : Result<StockSummaryDto>.Failure("Stock summary not found");
        }
    }
}
