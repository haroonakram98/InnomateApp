using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Stocks.Queries
{
    public class GetStockSummariesQuery : IRequest<Result<IEnumerable<StockSummaryDto>>>
    {
        public string? Search { get; set; }
    }

    public class GetStockSummariesQueryHandler : IRequestHandler<GetStockSummariesQuery, Result<IEnumerable<StockSummaryDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetStockSummariesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<StockSummaryDto>>> Handle(GetStockSummariesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<InnomateApp.Domain.Entities.StockSummary> summaries = await _uow.Stock.GetAllAsync();
            
            // Apply search filtering
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                summaries = summaries.Where(s => 
                    s.Product.Name.ToLower().Contains(search) || 
                    s.ProductId.ToString().Contains(search)
                );
            }

            var dtos = summaries.Select(ss => new StockSummaryDto
            {
                StockSummaryId = ss.StockSummaryId,
                ProductId = ss.ProductId,
                ProductName = ss.Product?.Name ?? string.Empty,
                TotalIn = ss.TotalIn,
                TotalOut = ss.TotalOut,
                Balance = ss.Balance,
                AverageCost = ss.AverageCost,
                TotalValue = ss.TotalValue,
                LastUpdated = ss.LastUpdated
            });

            return Result<IEnumerable<StockSummaryDto>>.Success(dtos);
        }
    }
}
