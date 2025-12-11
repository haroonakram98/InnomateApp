using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InnomateApp.Infrastructure.Repositories
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext context) : base(context) { }

        public async Task<Sale?> GetSaleWithDetailsAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.UsedBatches) // ✅ Include FIFO batches
                        .ThenInclude(b => b.PurchaseDetail)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.SaleId == id);
        }

        public async Task<IReadOnlyList<Sale>> GetSalesWithDetailsAsync()
        {
            try
            {
                return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.UsedBatches)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
            
        }

        public async Task AddSaleDetailAsync(SaleDetail detail)
        {
            await _context.SaleDetails.AddAsync(detail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSaleDetailAsync(SaleDetail detail)
        {
            _context.SaleDetails.Update(detail);
            await _context.SaveChangesAsync();
        }

        public async Task<SaleDetail?> GetSaleDetailAsync(int saleDetailId)
        {
            return await _context.SaleDetails
                .Include(sd => sd.Product)
                .Include(sd => sd.PurchaseDetail)
                .FirstOrDefaultAsync(sd => sd.SaleDetailId == saleDetailId);
        }

        public async Task<SaleDetail?> GetSaleDetailWithBatchesAsync(int saleDetailId)
        {
            try
            {
                var saleDetail = await _context.SaleDetails
                    .Include(sd => sd.Product) // Include product info
                    .Include(sd => sd.Sale) // Include parent sale
                    .Include(sd => sd.UsedBatches) // ✅ CRITICAL: Include FIFO batches
                        .ThenInclude(b => b.PurchaseDetail) // Include purchase detail for each batch
                            .ThenInclude(pd => pd.Purchase) // Include purchase info
                    .FirstOrDefaultAsync(sd => sd.SaleDetailId == saleDetailId);

                if (saleDetail == null)
                {
                    //_logger.LogWarning("SaleDetail {SaleDetailId} not found", saleDetailId);
                    return null;
                }

                //_logger.LogDebug(
                //    "Retrieved SaleDetail {SaleDetailId}: Product {ProductId}, " +
                //    "Quantity {Quantity}, UsedBatches: {BatchCount}",
                //    saleDetailId,
                //    saleDetail.ProductId,
                //    saleDetail.Quantity,
                //    saleDetail.UsedBatches.Count);

                return saleDetail;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error retrieving SaleDetail {SaleDetailId} with batches",
                //    saleDetailId);
                throw;
            }
        }

        // ========================================================================
        // ✅ BONUS: Additional Useful Methods
        // ========================================================================

        /// <summary>
        /// Gets all sale details for a sale with their FIFO batch information
        /// Useful for bulk operations like cancelling entire sales
        /// </summary>
        public async Task<IEnumerable<SaleDetail>> GetSaleDetailsWithBatchesBySaleIdAsync(int saleId)
        {
            try
            {
                var saleDetails = await _context.SaleDetails
                    .Include(sd => sd.Product)
                    .Include(sd => sd.Sale)
                    .Include(sd => sd.UsedBatches)
                        .ThenInclude(b => b.PurchaseDetail)
                            .ThenInclude(pd => pd.Purchase)
                    .Where(sd => sd.SaleId == saleId)
                    .ToListAsync();

                //_logger.LogDebug(
                //    "Retrieved {Count} SaleDetails for Sale {SaleId}",
                //    saleDetails.Count, saleId);

                return saleDetails;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error retrieving SaleDetails for Sale {SaleId}", saleId);
                throw;
            }
        }

        /// <summary>
        /// Gets a specific sale detail batch record
        /// </summary>
        public async Task<SaleDetailBatch?> GetSaleDetailBatchAsync(int saleDetailBatchId)
        {
            return await _context.SaleDetailBatches
                .Include(b => b.SaleDetail)
                    .ThenInclude(sd => sd.Product)
                .Include(b => b.PurchaseDetail)
                    .ThenInclude(pd => pd.Purchase)
                .FirstOrDefaultAsync(b => b.SaleDetailBatchId == saleDetailBatchId);
        }

        /// <summary>
        /// Gets all batches used in a specific sale
        /// Useful for audit reports showing which purchase batches contributed to a sale
        /// </summary>
        public async Task<IEnumerable<SaleDetailBatch>> GetBatchesUsedInSaleAsync(int saleId)
        {
            return await _context.SaleDetailBatches
                .Include(b => b.SaleDetail)
                    .ThenInclude(sd => sd.Product)
                .Include(b => b.PurchaseDetail)
                    .ThenInclude(pd => pd.Purchase)
                .Where(b => b.SaleDetail.SaleId == saleId)
                .OrderBy(b => b.PurchaseDetail.Purchase!.PurchaseDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all sales that used a specific purchase batch
        /// Useful for traceability - "which sales consumed this batch?"
        /// </summary>
        public async Task<IEnumerable<SaleDetailBatch>> GetSalesUsingPurchaseBatchAsync(
            int purchaseDetailId)
        {
            return await _context.SaleDetailBatches
                .Include(b => b.SaleDetail)
                    .ThenInclude(sd => sd.Sale)
                        .ThenInclude(s => s.Customer)
                .Include(b => b.PurchaseDetail)
                .Where(b => b.PurchaseDetailId == purchaseDetailId)
                .OrderBy(b => b.SaleDetail.Sale.SaleDate)
                .ToListAsync();
        }
        public async Task<string?> GetLastInvoiceNoAsync()
        {
            return await _context.Sales
                .OrderByDescending(s => s.SaleId)
                .Select(s => s.InvoiceNo)
                .FirstOrDefaultAsync();
        }
    }
}
