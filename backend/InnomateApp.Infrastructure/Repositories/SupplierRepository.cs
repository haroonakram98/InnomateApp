// Infrastructure/Repositories/SupplierRepository.cs
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Supplier>> GetSuppliersWithPurchasesAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Purchases)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierWithPurchasesAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Purchases)
                .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        public async Task<bool> SupplierExistsAsync(string name, string email)
        {
            return await _context.Suppliers
                .AnyAsync(s => s.Name == name || s.Email == email);
        }

        public async Task<bool> SupplierExistsAsync(int id, string name, string email)
        {
            return await _context.Suppliers
                .AnyAsync(s => s.SupplierId != id && (s.Name == name || s.Email == email));
        }

        public async Task<IReadOnlyList<Supplier>> GetActiveSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Supplier>> SearchSuppliersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _context.Suppliers
                .Where(s => s.Name.Contains(searchTerm) ||
                           s.Email.Contains(searchTerm) ||
                           s.Phone.Contains(searchTerm) ||
                           s.ContactPerson.Contains(searchTerm) ||
                           s.Address.Contains(searchTerm))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<decimal> GetSupplierTotalPurchaseAmountAsync(int supplierId)
        {
            return await _context.Purchases
                .Where(p => p.SupplierId == supplierId && p.Status == "Received")
                .SumAsync(p => p.TotalAmount);
        }

        public async Task<int> GetSupplierPurchaseCountAsync(int supplierId)
        {
            return await _context.Purchases
                .CountAsync(p => p.SupplierId == supplierId);
        }

        public async Task<int> GetSupplierPendingPurchaseCountAsync(int supplierId)
        {
            return await _context.Purchases
                .CountAsync(p => p.SupplierId == supplierId && p.Status == "Pending");
        }

        public async Task<DateTime?> GetSupplierLastPurchaseDateAsync(int supplierId)
        {
            return await _context.Purchases
                .Where(p => p.SupplierId == supplierId)
                .OrderByDescending(p => p.PurchaseDate)
                .Select(p => p.PurchaseDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Supplier>> GetSuppliersWithRecentPurchasesAsync(DateTime fromDate)
        {
            return await _context.Suppliers
                .Include(s => s.Purchases)
                .Where(s => s.Purchases.Any(p => p.PurchaseDate >= fromDate))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Supplier>> GetTopSuppliersByPurchaseAmountAsync(int count)
        {
            return await _context.Suppliers
                .Include(s => s.Purchases)
                .Select(s => new
                {
                    Supplier = s,
                    TotalAmount = s.Purchases
                        .Where(p => p.Status == "Received")
                        .Sum(p => p.TotalAmount)
                })
                .Where(x => x.TotalAmount > 0)
                .OrderByDescending(x => x.TotalAmount)
                .Take(count)
                .Select(x => x.Supplier)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}