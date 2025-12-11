using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Repositories
{
    public class StockRepository : GenericRepository<StockSummary>, IStockRepository
    {
        public StockRepository(AppDbContext context) : base(context) { }

        public async Task<StockSummary?> GetStockSummaryByProductIdAsync(int productId)
        {
            return await _context.StockSummaries
                .FirstOrDefaultAsync(ss => ss.ProductId == productId);
        }

        public async Task UpdateStockSummaryAsync(StockSummary stockSummary)
        {
            _context.StockSummaries.Update(stockSummary);
            await _context.SaveChangesAsync();
        }

        public async Task AddStockTransactionAsync(StockTransaction transaction)
        {
            await _context.StockTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<StockTransaction>> GetStockTransactionsByProductAsync(int productId)
        {
            return await _context.StockTransactions
                .Where(st => st.ProductId == productId)
                .OrderByDescending(st => st.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<PurchaseDetail>> GetAvailableBatchesForProductAsync(int productId)
        {
            return await _context.PurchaseDetails
                .Include(pd => pd.Purchase)
                .Where(pd => pd.ProductId == productId && pd.RemainingQty > 0)
                .OrderBy(pd => pd.Purchase!.PurchaseDate)
                .ThenBy(pd => pd.PurchaseDetailId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PurchaseDetail?> GetPurchaseDetailForFifoAsync(int purchaseDetailId)
        {
            return await _context.PurchaseDetails
                .Include(pd => pd.Product)
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == purchaseDetailId);
        }
        public async Task<PurchaseDetail?> GetPurchaseDetailByIdAsync(int purchaseDetailId)
        {
            return await _context.PurchaseDetails
                .Include(pd => pd.Purchase)
                .Include(pd => pd.Product)
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == purchaseDetailId);
        }

        // ========================================================================
        // ✅ CRITICAL: Update Purchase Detail (FIFO Deduction)
        // ========================================================================

        /// <summary>
        /// Updates a purchase detail - typically used to update RemainingQty after FIFO deduction
        /// </summary>
        public async Task UpdatePurchaseDetailAsync(PurchaseDetail purchaseDetail)
        {
            try
            {
                // Validate that RemainingQty doesn't go negative
                if (purchaseDetail.RemainingQty < 0)
                {
                    throw new InvalidOperationException(
                        $"RemainingQty cannot be negative for PurchaseDetail {purchaseDetail.PurchaseDetailId}. " +
                        $"Current value: {purchaseDetail.RemainingQty}");
                }

                // Validate that RemainingQty doesn't exceed original Quantity
                if (purchaseDetail.RemainingQty > purchaseDetail.Quantity)
                {
                    throw new InvalidOperationException(
                        $"RemainingQty ({purchaseDetail.RemainingQty}) cannot exceed " +
                        $"original Quantity ({purchaseDetail.Quantity}) for PurchaseDetail {purchaseDetail.PurchaseDetailId}");
                }

                // Mark as modified and save
                _context.PurchaseDetails.Update(purchaseDetail);
                await _context.SaveChangesAsync();

                //_logger.LogDebug(
                //    "Updated PurchaseDetail {Id}: Product {ProductId}, RemainingQty {Remaining}/{Total}",
                //    purchaseDetail.PurchaseDetailId,
                //    purchaseDetail.ProductId,
                //    purchaseDetail.RemainingQty,
                //    purchaseDetail.Quantity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //_logger.LogError(ex,
                //    "Concurrency error updating PurchaseDetail {Id}. " +
                //    "Another process may have modified this record.",
                //    purchaseDetail.PurchaseDetailId);
                throw new InvalidOperationException(
                    "Stock batch was modified by another transaction. Please retry.", ex);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error updating PurchaseDetail {Id}",
                //    purchaseDetail.PurchaseDetailId);
                throw;
            }
        }
        public async Task UpdatePurchaseDetailsBatchAsync(IEnumerable<PurchaseDetail> purchaseDetails)
        {
            try
            {
                var detailsList = purchaseDetails.ToList();

                // Validate all entries
                foreach (var detail in detailsList)
                {
                    if (detail.RemainingQty < 0)
                    {
                        throw new InvalidOperationException(
                            $"RemainingQty cannot be negative for PurchaseDetail {detail.PurchaseDetailId}");
                    }

                    if (detail.RemainingQty > detail.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"RemainingQty cannot exceed original Quantity for PurchaseDetail {detail.PurchaseDetailId}");
                    }
                }

                // Batch update
                _context.PurchaseDetails.UpdateRange(detailsList);
                await _context.SaveChangesAsync();

                //_logger.LogInformation(
                //    "Batch updated {Count} PurchaseDetails",
                //    detailsList.Count);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in batch update of PurchaseDetails");
                throw;
            }
        }

        public async Task<IEnumerable<PurchaseDetail>> GetPurchaseDetailsForReturnFIFOAsync(int productId)
        {
            return await _context.PurchaseDetails
                .Where(pd => pd.ProductId == productId)
                .OrderBy(pd => pd.Purchase!.PurchaseDate)
                .ThenBy(pd => pd.PurchaseDetailId)
                .ToListAsync();
        }
    }
}
