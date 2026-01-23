using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Queries
{
    public class GetProductStockBalanceQuery : IRequest<Result<decimal>>
    {
        public int ProductId { get; set; }

        public GetProductStockBalanceQuery(int productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductStockBalanceQueryHandler : IRequestHandler<GetProductStockBalanceQuery, Result<decimal>>
    {
        private readonly IStockService _stockService;

        public GetProductStockBalanceQueryHandler(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<Result<decimal>> Handle(GetProductStockBalanceQuery request, CancellationToken cancellationToken)
        {
            var balance = await _stockService.GetProductStockBalanceAsync(request.ProductId);
            return Result<decimal>.Success(balance);
        }
    }
}
