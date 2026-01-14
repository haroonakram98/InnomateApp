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
    public class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(AppDbContext context) : base(context) { }

        public async Task<Purchase?> GetPurchaseWithDetailsAsync(int id)
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesWithDetailsAsync()
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesBySupplierAsync(int supplierId)
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .Where(p => p.SupplierId == supplierId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PurchaseDetail?> GetPurchaseDetailAsync(int purchaseDetailId)
        {
            return await _context.PurchaseDetails
                .Include(pd => pd.Product)
                .Include(pd => pd.Purchase)
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == purchaseDetailId);
        }

        public async Task AddPurchaseDetailAsync(PurchaseDetail detail)
        {
            await _context.PurchaseDetails.AddAsync(detail);
        }

        public async Task UpdatePurchaseDetailAsync(PurchaseDetail detail)
        {
            _context.PurchaseDetails.Update(detail);
        }

        public async Task<IReadOnlyList<Purchase>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(pd => pd.Product)
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
                .OrderByDescending(p => p.PurchaseDate)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Dictionary<int, decimal>> GetProductCostsAsync(List<int> productIds)
        {
            return await _context.PurchaseDetails
                .Where(pd => productIds.Contains(pd.ProductId) && pd.RemainingQty > 0)
                .GroupBy(pd => pd.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    UnitCost = g.OrderByDescending(pd => pd.Purchase.PurchaseDate)
                               .ThenByDescending(pd => pd.PurchaseId)
                               .Select(pd => pd.UnitCost)
                               .FirstOrDefault()
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.UnitCost);
        }
    }
}
