// PurchasesController.cs

using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseResponseDto>> CreatePurchase(CreatePurchaseDto createDto)
        {
            var purchase = new Purchase
            {
                SupplierId = createDto.SupplierId,
                PurchaseDate = createDto.PurchaseDate,
                Notes = createDto.Notes,
                PurchaseDetails = createDto.PurchaseDetails.Select(detail => new PurchaseDetail
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost,
                    ExpiryDate = detail.ExpiryDate,
                    BatchNo = detail.BatchNo
                }).ToList()
            };

            var result = await _purchaseService.CreatePurchaseAsync(purchase);
            var responseDto = MapToResponseDto(result);

            return Ok(responseDto);
        }

        [HttpPost("{purchaseId}/receive")]
        public async Task<ActionResult<PurchaseResponseDto>> ReceivePurchase(int purchaseId)
        {
            var result = await _purchaseService.ReceivePurchaseAsync(purchaseId);
            var responseDto = MapToResponseDto(result);

            return Ok(responseDto);
        }

        [HttpPost("{purchaseId}/cancel")]
        public async Task<ActionResult> CancelPurchase(int purchaseId)
        {
            await _purchaseService.CancelPurchaseAsync(purchaseId);
            return Ok(new { message = "Purchase cancelled successfully" });
        }

        [HttpGet("{purchaseId}")]
        public async Task<ActionResult<PurchaseResponseDto>> GetPurchase(int purchaseId)
        {
            var purchase = await _purchaseService.GetPurchaseByIdAsync(purchaseId);
            if (purchase == null)
                return NotFound();

            var responseDto = MapToResponseDto(purchase);
            return Ok(responseDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseResponseDto>>> GetPurchases(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var purchases = await _purchaseService.GetPurchasesByDateRangeAsync(startDate.Value, endDate.Value);
            var responseDtos = purchases.Select(MapToResponseDto).ToList();

            return Ok(responseDtos);
        }

        private PurchaseResponseDto MapToResponseDto(Purchase purchase)
        {
            return new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                PurchaseNumber = purchase.InvoiceNo,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.Name,
                PurchaseDate = purchase.PurchaseDate,
                ReceivedDate = purchase.ReceivedDate,
                Status = purchase.Status,
                Notes = purchase.Notes,
                TotalAmount = purchase.TotalAmount,
                CreatedAt = purchase.CreatedAt,
                PurchaseDetails = purchase.PurchaseDetails.Select(detail => new PurchaseDetailResponseDto
                {
                    PurchaseDetailId = detail.PurchaseDetailId,
                    ProductId = detail.ProductId,
                    ProductName = detail.Product?.Name,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost,
                    TotalCost = detail.TotalCost,
                    RemainingQty = detail.RemainingQty,
                    ExpiryDate = detail.ExpiryDate,
                    BatchNo = detail.BatchNo
                }).ToList()
            };
        }
        [HttpGet("next-purchase-no")]
        public async Task<ActionResult<object>> GetNextPurchaseNumber()
        {
            var nextNo = await _purchaseService.GetNextPurchaseNumberAsync();
            return Ok(new { purchaseNo = nextNo });
        }

        [HttpGet("next-batch-no")]
        public async Task<ActionResult<object>> GetNextBatchNumber()
        {
            var nextNo = await _purchaseService.GetNextBatchNumberAsync();
            return Ok(new { batchNo = nextNo });
        }
    }
}