using InnomateApp.Application.DTOs.Returns.Requests;
using InnomateApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InnomateApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnsController : ControllerBase
    {
        private readonly IReturnService _returnService;

        public ReturnsController(IReturnService returnService)
        {
            _returnService = returnService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReturn([FromBody] CreateReturnRequest request)
        {
            try
            {
                var result = await _returnService.CreateReturnAsync(request);
                return CreatedAtAction(nameof(GetReturn), new { id = result.ReturnId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the return.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturn(int id)
        {
            var result = await _returnService.GetReturnByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("sale/{saleId}")]
        public async Task<IActionResult> GetReturnsBySaleId(int saleId)
        {
            var results = await _returnService.GetReturnsBySaleIdAsync(saleId);
            return Ok(results);
        }
    }
}
