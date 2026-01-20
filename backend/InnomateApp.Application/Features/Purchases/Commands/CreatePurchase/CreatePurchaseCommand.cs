using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Purchases.Commands.CreatePurchase
{
    public class CreatePurchaseCommand : IRequest<Result<int>>
    {
        public CreatePurchaseDto PurchaseDto { get; set; } = null!;
    }

    public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Result<int>>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreatePurchaseCommandHandler> _logger;

        public CreatePurchaseCommandHandler(
            IPurchaseRepository purchaseRepository,
            IStockRepository stockRepository,
            IMapper mapper,
            ILogger<CreatePurchaseCommandHandler> logger)
        {
            _purchaseRepository = purchaseRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var purchase = _mapper.Map<Purchase>(request.PurchaseDto);
                purchase.CreatedAt = DateTime.Now;
                purchase.TotalAmount = request.PurchaseDto.PurchaseDetails.Sum(d => d.Quantity * d.UnitCost);

                // Add purchase details
                foreach (var detailDto in request.PurchaseDto.PurchaseDetails)
                {
                    var purchaseDetail = _mapper.Map<PurchaseDetail>(detailDto);
                    purchaseDetail.RemainingQty = detailDto.Quantity; // Initially all quantity is available
                    purchaseDetail.TotalCost = detailDto.Quantity * detailDto.UnitCost;
                    purchase.PurchaseDetails.Add(purchaseDetail);
                }

                var createdPurchase = await _purchaseRepository.AddAsync(purchase);

                // Update stock summaries and create stock transactions
                await UpdateStockForPurchaseAsync(createdPurchase);

                return Result<int>.Success(createdPurchase.PurchaseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase");
                return Result<int>.Failure( "Error creating purchase" );
            }
        }

        private async Task UpdateStockForPurchaseAsync(Purchase purchase)
        {
            foreach (var detail in purchase.PurchaseDetails)
            {
                // Update or create stock summary
                var stockSummary = await _stockRepository.GetStockSummaryByProductIdAsync(detail.ProductId);
                if (stockSummary == null)
                {
                    stockSummary = new StockSummary
                    {
                        ProductId = detail.ProductId,
                        TotalIn = detail.Quantity,
                        TotalOut = 0,
                        Balance = detail.Quantity,
                        LastUpdated = DateTime.Now,
                        AverageCost = detail.UnitCost,
                        TotalValue = detail.TotalCost
                    };
                    await _stockRepository.AddAsync(stockSummary);
                }
                else
                {
                    // Update average cost using weighted average
                    var totalValue = stockSummary.Balance * stockSummary.AverageCost + detail.TotalCost;
                    var totalQuantity = stockSummary.Balance + detail.Quantity;
                    stockSummary.AverageCost = totalQuantity > 0 ? totalValue / totalQuantity : 0;

                    stockSummary.TotalIn += detail.Quantity;
                    stockSummary.Balance += detail.Quantity;
                    stockSummary.TotalValue = stockSummary.Balance * stockSummary.AverageCost;
                    stockSummary.LastUpdated = DateTime.Now;

                    await _stockRepository.UpdateStockSummaryAsync(stockSummary);
                }

                // Create stock transaction
                var stockTransaction = new StockTransaction
                {
                    ProductId = detail.ProductId,
                    TransactionType = 'I', // In
                    ReferenceId = purchase.PurchaseId,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost,
                    TotalCost = detail.TotalCost,
                    CreatedAt = DateTime.Now
                };
                await _stockRepository.AddStockTransactionAsync(stockTransaction);
            }
        }
    }
}
