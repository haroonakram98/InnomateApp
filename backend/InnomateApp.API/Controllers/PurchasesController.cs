using InnomateApp.Application.DTOs;
using InnomateApp.Application.Features.Purchases.Commands;
using InnomateApp.Application.Features.Purchases.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchase(CreatePurchaseDto createDto)
        {
            var result = await _mediator.Send(new CreatePurchaseCommand { CreateDto = createDto });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpPost("{purchaseId}/receive")]
        public async Task<IActionResult> ReceivePurchase(int purchaseId)
        {
            var result = await _mediator.Send(new ReceivePurchaseCommand(purchaseId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpPost("{purchaseId}/cancel")]
        public async Task<IActionResult> CancelPurchase(int purchaseId)
        {
            var result = await _mediator.Send(new CancelPurchaseCommand(purchaseId));
            return result.IsSuccess ? Ok(new { message = "Purchase cancelled successfully" }) : BadRequest(result.Error);
        }

        [HttpGet("{purchaseId}")]
        public async Task<IActionResult> GetPurchase(int purchaseId)
        {
            var result = await _mediator.Send(new GetPurchaseByIdQuery(purchaseId));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchases(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetPurchasesQuery { 
                StartDate = startDate, 
                EndDate = endDate,
                Search = search
            });
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        [HttpGet("next-purchase-no")]
        public async Task<IActionResult> GetNextPurchaseNumber()
        {
            var result = await _mediator.Send(new GetNextPurchaseNumberQuery());
            return result.IsSuccess ? Ok(new { purchaseNo = result.Data }) : BadRequest(result.Error);
        }

        [HttpGet("next-batch-no")]
        public async Task<IActionResult> GetNextBatchNumber()
        {
            var result = await _mediator.Send(new GetNextBatchNumberQuery());
            return result.IsSuccess ? Ok(new { batchNo = result.Data }) : BadRequest(result.Error);
        }
    }
}