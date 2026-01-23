using InnomateApp.Application.Common;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Queries
{
    public class GetProductStockValueQuery : IRequest<Result<decimal>>
    {
        public int ProductId { get; set; }

        public GetProductStockValueQuery(int productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductStockValueQueryHandler : IRequestHandler<GetProductStockValueQuery, Result<decimal>>
    {
        private readonly IStockService _stockService;

        public GetProductStockValueQueryHandler(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<Result<decimal>> Handle(GetProductStockValueQuery request, CancellationToken cancellationToken)
        {
            var value = await _stockService.GetProductStockValueAsync(request.ProductId);
            return Result<decimal>.Success(value);
        }
    }
}
