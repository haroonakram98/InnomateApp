using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Queries
{
    public class GetStockTransactionsQuery : IRequest<Result<IEnumerable<StockTransactionDto>>>
    {
        public int ProductId { get; set; }

        public GetStockTransactionsQuery(int productId)
        {
            ProductId = productId;
        }
    }

    public class GetStockTransactionsQueryHandler : IRequestHandler<GetStockTransactionsQuery, Result<IEnumerable<StockTransactionDto>>>
    {
        private readonly IUnitOfWork _uow;

        public GetStockTransactionsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<IEnumerable<StockTransactionDto>>> Handle(GetStockTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _uow.Stock.GetStockTransactionsByProductAsync(request.ProductId);
            
            var dtos = transactions.Select(t => new StockTransactionDto
            {
                StockTransactionId = t.StockTransactionId,
                TransactionId = t.TransactionId,
                ProductId = t.ProductId,
                ProductName = t.Product?.Name ?? string.Empty,
                TransactionType = t.TransactionType.ToString(),
                ReferenceId = t.ReferenceId,
                Quantity = t.Quantity,
                UnitCost = t.UnitCost,
                TotalCost = t.TotalCost,
                CreatedAt = t.CreatedAt
            });

            return Result<IEnumerable<StockTransactionDto>>.Success(dtos);
        }
    }
}
