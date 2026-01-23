// API/Controllers/SuppliersController.cs
using InnomateApp.Application.DTOs;
using InnomateApp.Application.DTOs.Requests;
using InnomateApp.Application.Features.Suppliers.Commands;
using InnomateApp.Application.Features.Suppliers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SuppliersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers([FromQuery] string search = null)
        {
            var result = await _mediator.Send(new GetSuppliersQuery { SearchTerm = search });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSuppliers()
        {
            var result = await _mediator.Send(new GetSuppliersQuery { ActiveOnly = true });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopSuppliers([FromQuery] int count = 10)
        {
            var result = await _mediator.Send(new GetTopSuppliersQuery { Count = count });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetSuppliersWithRecentPurchases([FromQuery] int days = 30)
        {
            var result = await _mediator.Send(new GetSuppliersWithRecentPurchasesQuery { Days = days });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var result = await _mediator.Send(new GetSupplierByIdQuery { Id = id });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{id}/with-purchases")]
        public async Task<IActionResult> GetSupplierWithPurchases(int id)
        {
            var result = await _mediator.Send(new GetSupplierWithPurchasesQuery { Id = id });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetSupplierStats(int id)
        {
            var result = await _mediator.Send(new GetSupplierStatsQuery { Id = id });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
        {
            var result = await _mediator.Send(new CreateSupplierCommand { SupplierDto = dto });

            if (!result.IsSuccess)
            {
                if (result.Errors?.Count > 0)
                    return BadRequest(new { error = result.Error, validationErrors = result.Errors });

                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = result.Data.SupplierId }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierDto dto)
        {
            var result = await _mediator.Send(new UpdateSupplierCommand { Id = id, SupplierDto = dto });

            if (!result.IsSuccess)
            {
                if (result.Errors?.Count > 0)
                    return BadRequest(new { error = result.Error, validationErrors = result.Errors });

                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var result = await _mediator.Send(new DeleteSupplierCommand { Id = id });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(new { message = "Supplier deleted successfully" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleSupplierStatus(int id)
        {
            var result = await _mediator.Send(new ToggleSupplierStatusCommand { Id = id });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(new
            {
                message = $"Supplier {(result.Data ? "activated" : "deactivated")} successfully",
                isActive = result.Data
            });
        }
    }
}