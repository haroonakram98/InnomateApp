using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.DTOs.Sales.Requests;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Sales.Commands.CreateSale
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSaleCommandHandler> _logger;

        public CreateSaleCommandHandler(
            IUnitOfWork unitOfWork,
            IStockService stockService,
            IMapper mapper,
            ILogger<CreateSaleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate stock availability
            var validationItems = request.SaleDto.SaleDetails
                .Select(d => (d.ProductId, d.Quantity))
                .ToList();

            var validationResult = await _stockService.ValidateStockAvailabilityAsync(validationItems);
            if (!validationResult.IsValid)
            {
                return Result<int>.Failure(string.Join("; ", validationResult.Errors));
            }

            // 2. Create sale entity (Initial Save to get IDs)
            var sale = _mapper.Map<Sale>(request.SaleDto);
            sale.CreatedAt = DateTime.Now;
            sale.SaleDate = DateTime.Now;
            
            // Calculate initial totals (Price based)
            sale.TotalAmount = request.SaleDto.SaleDetails.Sum(d => d.Quantity * d.UnitPrice);
            sale.BalanceAmount = sale.TotalAmount;
            sale.IsFullyPaid = false;
            sale.PaidAmount = 0;
            
            // Add details
            sale.SaleDetails = request.SaleDto.SaleDetails.Select(detailDto => 
            {
                var detail = _mapper.Map<SaleDetail>(detailDto);
                detail.Total = detailDto.Quantity * detailDto.UnitPrice;
                return detail;
            }).ToList();

            // Save to generate SaleId and SaleDetailIds
            var createdSale = await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            // 3. Process FIFO for each detail
            decimal totalCost = 0;

            foreach (var detail in createdSale.SaleDetails)
            {
                var fifoResult = await _stockService.ProcessSaleWithFIFOAsync(
                    detail.ProductId,
                    detail.Quantity,
                    detail.SaleDetailId,
                    $"INV-{createdSale.InvoiceNo}",
                    $"Sold via Invoice #{createdSale.InvoiceNo}");

                if (!fifoResult.Success)
                {
                    throw new InvalidOperationException($"FIFO processing failed for Product {detail.ProductId}: {fifoResult.Message}");
                }

                // Accumulate cost
                totalCost += fifoResult.TotalCost;
            }

            // 4. Update Sale with Cost and Profit
            createdSale.TotalCost = totalCost;
            createdSale.TotalProfit = createdSale.TotalAmount - totalCost;
            createdSale.ProfitMargin = createdSale.TotalAmount > 0 
                ? (createdSale.TotalProfit / createdSale.TotalAmount) * 100 
                : 0;

            await _unitOfWork.Sales.UpdateAsync(createdSale);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Sale created successfully with ID: {SaleId}", createdSale.SaleId);
            return Result<int>.Success(createdSale.SaleId);
        }
    }
}
