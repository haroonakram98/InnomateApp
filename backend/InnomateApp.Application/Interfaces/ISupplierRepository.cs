// Application/Interfaces/IRepositories/ISupplierRepository.cs
using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        Task<IReadOnlyList<Supplier>> GetSuppliersWithPurchasesAsync();
        Task<Supplier?> GetSupplierWithPurchasesAsync(int id);
        Task<bool> SupplierExistsAsync(string name, string email);
        Task<bool> SupplierExistsAsync(int id, string name, string email);
        Task<IReadOnlyList<Supplier>> GetActiveSuppliersAsync();
        Task<IReadOnlyList<Supplier>> SearchSuppliersAsync(string searchTerm);
        Task<decimal> GetSupplierTotalPurchaseAmountAsync(int supplierId);
        Task<int> GetSupplierPurchaseCountAsync(int supplierId);
        Task<int> GetSupplierPendingPurchaseCountAsync(int supplierId);
        Task<DateTime?> GetSupplierLastPurchaseDateAsync(int supplierId);

        // Additional methods for enhanced functionality
        Task<IReadOnlyList<Supplier>> GetSuppliersWithRecentPurchasesAsync(DateTime fromDate);
        Task<IReadOnlyList<Supplier>> GetTopSuppliersByPurchaseAmountAsync(int count);
    }
}