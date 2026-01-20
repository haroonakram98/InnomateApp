using InnomateApp.Application.DTOs.Products.Requests;
using InnomateApp.Application.Features.Products.Commands.CreateProduct;
using InnomateApp.Application.Features.Products.Commands.DeleteProduct;
using InnomateApp.Application.Features.Products.Commands.UpdateProduct;
using InnomateApp.Application.Features.Products.Queries.GetAllProducts;
using InnomateApp.Application.Features.Products.Queries.GetProductById;
using InnomateApp.Application.Features.Products.Queries.GetProductLookup;
using InnomateApp.Application.Features.Products.Queries.GetProductsForSale;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            
             if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var result = await _mediator.Send(new CreateProductCommand(request));
            
             if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.ProductId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            if (id != request.ProductId) return BadRequest(new { error = "ID mismatch" });
            
            var result = await _mediator.Send(new UpdateProductCommand(request));
            
             if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            
             if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return NoContent();
        }

        [HttpGet("for-sale")]
        public async Task<IActionResult> ForSales()
        {
            var products = await _mediator.Send(new GetProductsForSaleQuery());
            return Ok(products);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookup()
        {
            var products = await _mediator.Send(new GetProductLookupQuery());
            return Ok(products);
        }
    }
}

