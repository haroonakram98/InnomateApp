using InnomateApp.Application.DTOs.Sales.Requests;
using InnomateApp.Application.Interfaces.Services;
using MediatR;
using InnomateApp.Application.Features.Sales.Commands.CreateSale;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly IMediator _mediator;

        public SalesController(ISaleService saleService, IMediator mediator)
        {
            _saleService = saleService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _saleService.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _saleService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var command = new CreateSaleCommand { SaleDto = request };
            var result = await _mediator.Send(command);


            if (result.IsSuccess)
            {
               return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
            }
            return BadRequest(result.Error);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSaleRequest request)
        {
            if (id != request.SaleId) return BadRequest("ID mismatch");
            var updated = await _saleService.UpdateAsync(request);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _saleService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("next-invoice-no")]
        public async Task<IActionResult> GetNextInvoiceNumber()
        {
            var nextInvoiceNo = await _saleService.GetNextInvoiceNumberAsync();
            return Ok(new { invoiceNo = nextInvoiceNo });
        }

        [HttpPost("{id:int}/payments")]
        public async Task<IActionResult> AddPayment(int id, [FromBody] AddPaymentRequest request)
        {
            if (id != request.SaleId) return BadRequest("Sale ID mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var result = await _saleService.AddPaymentAsync(request);
            return Ok(result);
        }
    }
}
