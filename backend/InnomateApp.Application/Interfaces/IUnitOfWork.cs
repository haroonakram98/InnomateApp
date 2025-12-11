using InnomateApp.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISupplierRepository Suppliers { get; }
        IProductRepository Products { get; }
        IPurchaseRepository Purchases { get; }
        IStockRepository Stock { get; }
        ISaleRepository Sales { get; }

        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<ITransaction> BeginTransactionAsync(); // Use application-specific interface
    }
}