using InnomateApp.Application.DTOs;
using InnomateApp.Application.Features.Products.Commands;
using InnomateApp.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetProductsQuery { Search = search });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _mediator.Send(new CreateProductCommand { CreateDto = dto });
            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Data.ProductId }, result.Data) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.ProductId) return BadRequest("Property ID mismatch");
            
            var result = await _mediator.Send(new UpdateProductCommand { UpdateDto = dto });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeactivateProductCommand(id));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpGet("for-sale")]
        public async Task<IActionResult> GetForSale()
        {
            var result = await _mediator.Send(new GetProductsForSaleQuery());
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }
    }
}
