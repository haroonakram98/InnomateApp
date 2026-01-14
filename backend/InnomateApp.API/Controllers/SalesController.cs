using InnomateApp.Application.DTOs.Sales.Requests;
using InnomateApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
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
            var created = await _saleService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.SaleId }, created);
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
