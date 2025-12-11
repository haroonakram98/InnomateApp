// StocksController.cs
using Microsoft.AspNetCore.Mvc;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IStockService stockService, ILogger<StocksController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        // GET: api/stocks/summary/{productId}
        [HttpGet("summary/{productId}")]
        public async Task<ActionResult<StockSummaryDto>> GetStockSummary(int productId)
        {
            try
            {
                var summary = await _stockService.GetStockSummaryByProductIdAsync(productId);
                if (summary == null)
                    return NotFound($"Stock summary not found for product ID {productId}");

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock summary for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving stock summary");
            }
        }

        // GET: api/stocks/summary
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<StockSummaryDto>>> GetAllStockSummaries()
        {
            try
            {
                var summaries = await _stockService.GetAllStockSummariesAsync();
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stock summaries");
                return StatusCode(500, "An error occurred while retrieving stock summaries");
            }
        }

        // GET: api/stocks/transactions/{productId}
        [HttpGet("transactions/{productId}")]
        public async Task<ActionResult<IEnumerable<StockTransactionDto>>> GetStockTransactions(int productId)
        {
            try
            {
                var transactions = await _stockService.GetStockTransactionsByProductAsync(productId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock transactions for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving stock transactions");
            }
        }

        // GET: api/stocks/batches/{productId}
        [HttpGet("batches/{productId}")]
        public async Task<ActionResult<IEnumerable<FifoBatchDto>>> GetAvailableBatches(int productId)
        {
            try
            {
                var batches = await _stockService.GetAvailableBatchesAsync(productId);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available batches for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving available batches");
            }
        }

        // POST: api/stocks/movement
        [HttpPost("movement")]
        public async Task<ActionResult> RecordStockMovement([FromBody] StockMovementDto movement)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate transaction type
                if (movement.TransactionType != "P" && movement.TransactionType != "S" && movement.TransactionType != "A")
                    return BadRequest("Transaction type must be 'P' (Purchase), 'S' (Sale), or 'A' (Adjustment)");

                var success = await _stockService.RecordStockMovementAsync(movement);
                if (!success)
                    return BadRequest("Failed to record stock movement");

                return Ok(new { message = "Stock movement recorded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording stock movement");
                return StatusCode(500, "An error occurred while recording stock movement");
            }
        }

        // POST: api/stocks/sale/fifo
        [HttpPost("sale/fifo")]
        public async Task<ActionResult<FIFOSaleResultDto>> ProcessFIFOSale([FromBody] FIFOSaleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _stockService.ProcessFIFOSaleAsync(
                    request.ProductId,
                    request.Quantity,
                    request.SaleReferenceId);

                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing FIFO sale");
                return StatusCode(500, "An error occurred while processing FIFO sale");
            }
        }

        // GET: api/stocks/balance/{productId}
        [HttpGet("balance/{productId}")]
        public async Task<ActionResult<decimal>> GetStockBalance(int productId)
        {
            try
            {
                var balance = await _stockService.GetProductStockBalanceAsync(productId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock balance for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving stock balance");
            }
        }

        // GET: api/stocks/value/{productId}
        [HttpGet("value/{productId}")]
        public async Task<ActionResult<decimal>> GetStockValue(int productId)
        {
            try
            {
                var value = await _stockService.GetProductStockValueAsync(productId);
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock value for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving stock value");
            }
        }

        // PUT: api/stocks/summary/{productId}/refresh
        [HttpPut("summary/{productId}/refresh")]
        public async Task<ActionResult> RefreshStockSummary(int productId)
        {
            try
            {
                var success = await _stockService.UpdateStockSummaryAsync(productId);
                if (!success)
                    return BadRequest("Failed to refresh stock summary");

                return Ok(new { message = "Stock summary refreshed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing stock summary for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while refreshing stock summary");
            }
        }
    }

    // Request DTO for FIFO sale
    public class FIFOSaleRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required]
        public int SaleReferenceId { get; set; }
    }
}