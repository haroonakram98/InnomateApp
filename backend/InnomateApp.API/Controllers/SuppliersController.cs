// API/Controllers/SuppliersController.cs
using InnomateApp.Application.DTOs.Requests;
using InnomateApp.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers([FromQuery] string search = null)
        {
            var result = string.IsNullOrEmpty(search)
                ? await _supplierService.GetAllSuppliersAsync()
                : await _supplierService.SearchSuppliersAsync(search);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error, validationErrors = result.Errors });

            return Ok(result.Data);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSuppliers()
        {
            var result = await _supplierService.GetActiveSuppliersAsync();

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopSuppliers([FromQuery] int count = 10)
        {
            var result = await _supplierService.GetTopSuppliersAsync(count);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetSuppliersWithRecentPurchases([FromQuery] int days = 30)
        {
            var result = await _supplierService.GetSuppliersWithRecentPurchasesAsync(days);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var result = await _supplierService.GetSupplierByIdAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{id}/with-purchases")]
        public async Task<IActionResult> GetSupplierWithPurchases(int id)
        {
            var result = await _supplierService.GetSupplierWithPurchasesAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetSupplierStats(int id)
        {
            var result = await _supplierService.GetSupplierStatsAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _supplierService.CreateSupplierAsync(request);

            if (!result.IsSuccess)
            {
                if (result.Errors?.Count > 0)
                    return BadRequest(new { error = result.Error, validationErrors = result.Errors });

                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetSupplier), new { id = result.Data.SupplierId }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] UpdateSupplierRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _supplierService.UpdateSupplierAsync(id, request);

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
            var result = await _supplierService.DeleteSupplierAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new { error = result.Error });

            return Ok(new { message = "Supplier deleted successfully" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleSupplierStatus(int id)
        {
            var result = await _supplierService.ToggleSupplierStatusAsync(id);

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