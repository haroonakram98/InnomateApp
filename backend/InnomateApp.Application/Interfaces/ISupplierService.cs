// Application/Interfaces/IServices/ISupplierService.cs
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.DTOs.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces.IServices
{
    public interface ISupplierService
    {
        Task<Result<SupplierDto>> CreateSupplierAsync(CreateSupplierRequest request);
        Task<Result<SupplierDto>> UpdateSupplierAsync(int id, UpdateSupplierRequest request);
        Task<Result<bool>> DeleteSupplierAsync(int id);
        Task<Result<SupplierDto>> GetSupplierByIdAsync(int id);
        Task<Result<SupplierDetailDto>> GetSupplierWithPurchasesAsync(int id);
        Task<Result<IEnumerable<SupplierDto>>> GetAllSuppliersAsync();
        Task<Result<IEnumerable<SupplierDto>>> GetActiveSuppliersAsync();
        Task<Result<IEnumerable<SupplierDto>>> SearchSuppliersAsync(string searchTerm);
        Task<Result<bool>> ToggleSupplierStatusAsync(int id);
        Task<Result<SupplierStatsResponse>> GetSupplierStatsAsync(int supplierId);
        Task<Result<IEnumerable<SupplierDto>>> GetTopSuppliersAsync(int count = 10);
        Task<Result<IEnumerable<SupplierDto>>> GetSuppliersWithRecentPurchasesAsync(int days = 30);
    }

    public class SupplierStatsResponse
    {
        public int TotalPurchases { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int PendingPurchases { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public decimal AveragePurchaseAmount { get; set; }
        public int CompletedPurchases { get; set; }
    }
}