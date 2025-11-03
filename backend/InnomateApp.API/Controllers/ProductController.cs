using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok(new { Message = "Product created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.ProductId) return BadRequest();
            await _service.UpdateAsync(dto);
            return Ok(new { Message = "Product updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { Message = "Product deleted successfully" });
        }
    }
}
