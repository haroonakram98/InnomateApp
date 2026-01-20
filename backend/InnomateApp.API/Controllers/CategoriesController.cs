using InnomateApp.Application.DTOs.Categories.Requests;
using InnomateApp.Application.Features.Categories.Commands.CreateCategory;
using InnomateApp.Application.Features.Categories.Commands.DeleteCategory;
using InnomateApp.Application.Features.Categories.Commands.UpdateCategory;
using InnomateApp.Application.Features.Categories.Queries.GetAllCategories;
using InnomateApp.Application.Features.Categories.Queries.GetCategoryById;
using InnomateApp.Application.Features.Categories.Queries.GetCategoryLookup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    /// <summary>
    /// Controller for Category CRUD operations
    /// Uses thin controller pattern - all logic delegated to MediatR handlers
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all categories with product counts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(categories);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));
            
            if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }
            
            return Ok(result.Data);
        }

        /// <summary>
        /// Get lightweight category list for dropdowns
        /// </summary>
        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookup()
        {
            var categories = await _mediator.Send(new GetCategoryLookupQuery());
            return Ok(categories);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var result = await _mediator.Send(new CreateCategoryCommand(request));

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.CategoryId }, result.Data);
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            if (id != request.CategoryId)
            {
                return BadRequest(new { error = "ID mismatch" });
            }

            var result = await _mediator.Send(new UpdateCategoryCommand(request));

            if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));

            if (!result.IsSuccess)
            {
                return result.StatusCode == 404 
                    ? NotFound(new { error = result.Error }) 
                    : BadRequest(new { error = result.Error });
            }

            return NoContent();
        }
    }
}

