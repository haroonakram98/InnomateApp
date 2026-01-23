using InnomateApp.Application.DTOs;
using InnomateApp.Application.Features.Categories.Commands;
using InnomateApp.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetCategoriesQuery { Search = search });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (!result.IsSuccess)
                return result.Error.Contains("not found") ? NotFound() : BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _mediator.Send(new CreateCategoryCommand { CategoryDto = dto });
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.CategoryId }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var result = await _mediator.Send(new UpdateCategoryCommand { Id = id, CategoryDto = dto });
            if (!result.IsSuccess)
                return result.Error.Contains("not found") ? NotFound() : BadRequest(result.Error);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            if (!result.IsSuccess)
                return result.Error.Contains("not found") ? NotFound() : BadRequest(result.Error);

            return NoContent();
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _mediator.Send(new ToggleCategoryStatusCommand(id));
            if (!result.IsSuccess)
                return result.Error.Contains("not found") ? NotFound() : BadRequest(result.Error);

            return Ok(new { isActive = result.Data });
        }
    }
}
