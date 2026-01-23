using Microsoft.AspNetCore.Mvc;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Features.Stocks.Queries;
using InnomateApp.Application.Features.Stocks.Commands;
using MediatR;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IMediator mediator, ILogger<StocksController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: api/stocks/summary/{productId}
        [HttpGet("summary/{productId}")]
        public async Task<IActionResult> GetStockSummary(int productId)
        {
            var result = await _mediator.Send(new GetStockSummaryByIdQuery(productId));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        // GET: api/stocks/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetAllStockSummaries([FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetStockSummariesQuery { Search = search });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // GET: api/stocks/transactions/{productId}
        [HttpGet("transactions/{productId}")]
        public async Task<IActionResult> GetStockTransactions(int productId)
        {
            var result = await _mediator.Send(new GetStockTransactionsQuery(productId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // GET: api/stocks/batches/{productId}
        [HttpGet("batches/{productId}")]
        public async Task<IActionResult> GetAvailableBatches(int productId)
        {
            var result = await _mediator.Send(new GetAvailableBatchesQuery(productId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // POST: api/stocks/movement
        [HttpPost("movement")]
        public async Task<IActionResult> RecordStockMovement([FromBody] StockMovementDto movement)
        {
            if (movement.TransactionType != "P" && movement.TransactionType != "S" && movement.TransactionType != "A")
                return BadRequest("Transaction type must be 'P' (Purchase), 'S' (Sale), or 'A' (Adjustment)");

            var result = await _mediator.Send(new RecordStockMovementCommand { Movement = movement });
            return result.IsSuccess ? Ok(new { message = "Stock movement recorded successfully" }) : BadRequest(result.Error);
        }

        // POST: api/stocks/sale/fifo
        [HttpPost("sale/fifo")]
        public async Task<IActionResult> ProcessFIFOSale([FromBody] FIFOSaleRequestDto request)
        {
            var result = await _mediator.Send(new ProcessFIFOSaleCommand { Request = request });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // GET: api/stocks/balance/{productId}
        [HttpGet("balance/{productId}")]
        public async Task<IActionResult> GetStockBalance(int productId)
        {
            var result = await _mediator.Send(new GetProductStockBalanceQuery(productId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // GET: api/stocks/value/{productId}
        [HttpGet("value/{productId}")]
        public async Task<IActionResult> GetStockValue(int productId)
        {
            var result = await _mediator.Send(new GetProductStockValueQuery(productId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // PUT: api/stocks/summary/{productId}/refresh
        [HttpPut("summary/{productId}/refresh")]
        public async Task<IActionResult> RefreshStockSummary(int productId)
        {
            var result = await _mediator.Send(new RefreshStockSummaryCommand(productId));
            return result.IsSuccess ? Ok(new { message = "Stock summary refreshed successfully" }) : BadRequest(result.Error);
        }
    }
}