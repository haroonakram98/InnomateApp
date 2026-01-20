using InnomateApp.Application.DTOs.Suppliers.Requests;
using InnomateApp.Application.Features.Suppliers.Commands.CreateSupplier;
using InnomateApp.Application.Features.Suppliers.Commands.DeleteSupplier;
using InnomateApp.Application.Features.Suppliers.Commands.UpdateSupplier;
using InnomateApp.Application.Features.Suppliers.Commands.ToggleSupplierStatus;
using InnomateApp.Application.Features.Suppliers.Queries.GetActiveSuppliers;
using InnomateApp.Application.Features.Suppliers.Queries.GetAllSuppliers;
using InnomateApp.Application.Features.Suppliers.Queries.GetSupplierById;
using InnomateApp.Application.Features.Suppliers.Queries.GetSupplierLookup;
using InnomateApp.Application.Features.Suppliers.Queries.GetSupplierWithPurchases;
using InnomateApp.Application.Features.Suppliers.Queries.GetSupplierStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var result = await _mediator.Send(new GetAllSuppliersQuery(search));
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSuppliers()
        {
            var result = await _mediator.Send(new GetActiveSuppliersQuery());
            return Ok(result);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookup()
        {
            var result = await _mediator.Send(new GetSupplierLookupQuery());
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var result = await _mediator.Send(new GetSupplierByIdQuery(id));

            if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}/with-purchases")]
        public async Task<IActionResult> GetSupplierWithPurchases(int id)
        {
            var result = await _mediator.Send(new GetSupplierWithPurchasesQuery(id));

            if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}/stats")]
        public async Task<IActionResult> GetSupplierStats(int id)
        {
            var result = await _mediator.Send(new GetSupplierStatsQuery(id));

            if (!result.IsSuccess)
            {
                 return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
        {
            var result = await _mediator.Send(new CreateSupplierCommand(request));

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = result.Data.SupplierId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierRequest request)
        {
            if (id != request.SupplierId) return BadRequest(new { error = "ID mismatch" });

            var result = await _mediator.Send(new UpdateSupplierCommand(request));

            if (!result.IsSuccess)
            {
                 return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var result = await _mediator.Send(new DeleteSupplierCommand(id));

            if (!result.IsSuccess)
            {
                 return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleSupplierStatus(int id)
        {
            var result = await _mediator.Send(new ToggleSupplierStatusCommand(id));

            if (!result.IsSuccess)
            {
                 return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(new
            {
                message = $"Supplier {(result.Data ? "activated" : "deactivated")} successfully",
                isActive = result.Data
            });
        }
    }
}