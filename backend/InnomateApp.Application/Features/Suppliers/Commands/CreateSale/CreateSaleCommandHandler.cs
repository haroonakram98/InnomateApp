using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Suppliers.Commands.CreateSale
{
    

    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFifoService _fifoService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSaleCommandHandler> _logger;

        public CreateSaleCommandHandler(
            IUnitOfWork unitOfWork,
            IFifoService fifoService,
            IMapper mapper,
            ILogger<CreateSaleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _fifoService = fifoService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validate stock availability and calculate FIFO costs
                var saleCalculation = await CalculateSaleWithFifoAsync(request.SaleDto);
                if (!saleCalculation.Success)
                    return Result<int>.Failure(saleCalculation.ErrorMessage!);

                // 2. Create sale entity
                var sale = _mapper.Map<Sale>(request.SaleDto);
                sale.CreatedAt = DateTime.Now;
                sale.TotalAmount = saleCalculation.TotalAmount;
                sale.BalanceAmount = sale.TotalAmount;
                sale.IsFullyPaid = false;
                sale.PaidAmount = 0;

                // 3. Create sale details with FIFO allocations
                foreach (var detailDto in request.SaleDto.SaleDetails)
                {
                    var saleDetail = _mapper.Map<SaleDetail>(detailDto);
                    saleDetail.Total = detailDto.Quantity * detailDto.UnitPrice;

                    // Allocate stock using FIFO
                    var allocation = await _fifoService.AllocateStockForSaleAsync(
                        detailDto.ProductId, detailDto.Quantity);

                    if (allocation.Success && allocation.Allocations.Any())
                    {
                        saleDetail.PurchaseDetailId = allocation.Allocations.First().PurchaseDetailId;
                    }
                    else
                    {
                        return Result<int>.Failure($"Failed to allocate stock for product {detailDto.ProductId}" );
                    }

                    sale.SaleDetails.Add(saleDetail);
                }

                // 4. Save sale (transaction handled by behavior)
                var createdSale = await _unitOfWork.Sales.AddAsync(sale);

                // 5. Update stock levels
                await UpdateStockForSaleAsync(createdSale, saleCalculation);

                _logger.LogInformation("Sale created successfully with ID: {SaleId}", createdSale.SaleId);
                return Result<int>.Success(createdSale.SaleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return Result<int>.Failure( "Error creating sale" );
            }
        }

        private async Task<SaleCalculationResultDto> CalculateSaleWithFifoAsync(CreateSaleDto saleDto)
        {
            var result = new SaleCalculationResultDto();
            decimal totalAmount = 0;
            decimal totalCost = 0;

            foreach (var detail in saleDto.SaleDetails)
            {
                // Check stock availability
                var stockSummary = await _unitOfWork.Stock.GetStockSummaryByProductIdAsync(detail.ProductId);
                if (stockSummary == null || stockSummary.Balance < detail.Quantity)
                {
                    return new SaleCalculationResultDto
                    {
                        Success = false,
                        ErrorMessage = $"Insufficient stock for product {detail.ProductId}"
                    };
                }

                // Calculate FIFO cost
                var fifoCost = await _fifoService.CalculateFifoCostAsync(detail.ProductId, detail.Quantity);
                if (fifoCost == 0)
                {
                    return new SaleCalculationResultDto
                    {
                        Success = false,
                        ErrorMessage = $"Error calculating cost for product {detail.ProductId}"
                    };
                }

                var detailAmount = detail.Quantity * detail.UnitPrice;
                totalAmount += detailAmount;
                totalCost += fifoCost;

                result.DetailCosts.Add(new SaleDetailCostDto
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    UnitCost = fifoCost / detail.Quantity,
                    TotalCost = fifoCost,
                    Profit = detailAmount - fifoCost
                });
            }

            result.TotalAmount = totalAmount;
            result.TotalCost = totalCost;
            result.GrossProfit = totalAmount - totalCost;
            result.ProfitMargin = totalAmount > 0 ? (result.GrossProfit / totalAmount) * 100 : 0;
            result.Success = true;

            return result;
        }

        private async Task UpdateStockForSaleAsync(Sale sale, SaleCalculationResultDto calculation)
        {
            foreach (var detail in sale.SaleDetails)
            {
                var productCalculation = calculation.DetailCosts
                    .FirstOrDefault(dc => dc.ProductId == detail.ProductId);

                if (productCalculation == null) continue;

                // Update stock summary
                var stockSummary = await _unitOfWork.Stock.GetStockSummaryByProductIdAsync(detail.ProductId);
                if (stockSummary != null)
                {
                    stockSummary.TotalOut += detail.Quantity;
                    stockSummary.Balance -= detail.Quantity;
                    stockSummary.TotalValue = stockSummary.Balance * stockSummary.AverageCost;
                    stockSummary.LastUpdated = DateTime.Now;

                    await _unitOfWork.Stock.UpdateStockSummaryAsync(stockSummary);
                }

                // Update purchase detail remaining quantity (FIFO allocation)
                if (detail.PurchaseDetailId.HasValue)
                {
                    var purchaseDetail = await _unitOfWork.Stock.GetPurchaseDetailForFifoAsync(detail.PurchaseDetailId.Value);
                    if (purchaseDetail != null)
                    {
                        purchaseDetail.RemainingQty -= detail.Quantity;
                        await _unitOfWork.Purchases.UpdatePurchaseDetailAsync(purchaseDetail);
                    }
                }

                // Create stock transaction
                var stockTransaction = new StockTransaction
                {
                    ProductId = detail.ProductId,
                    TransactionType = 'O',
                    ReferenceId = sale.SaleId,
                    Quantity = detail.Quantity,
                    UnitCost = productCalculation.UnitCost,
                    TotalCost = productCalculation.TotalCost,
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.Stock.AddStockTransactionAsync(stockTransaction);
            }
        }
    }

}
