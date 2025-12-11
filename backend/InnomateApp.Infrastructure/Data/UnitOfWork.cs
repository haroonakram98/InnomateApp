using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Infrastructure.Persistence;
using InnomateApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Suppliers = new SupplierRepository(_context);
            Products = new ProductRepository(_context);
            Purchases = new PurchaseRepository(_context);
            Stock = new StockRepository(_context);
            Sales = new SaleRepository(_context);
        }

        public ISupplierRepository Suppliers { get; }
        public IProductRepository Products { get; }
        public IPurchaseRepository Purchases { get; }
        public IStockRepository Stock { get; }
        public ISaleRepository Sales { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ITransaction> BeginTransactionAsync()
        {
            var efTransaction = await _context.Database.BeginTransactionAsync();
            return new Transaction(efTransaction);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
