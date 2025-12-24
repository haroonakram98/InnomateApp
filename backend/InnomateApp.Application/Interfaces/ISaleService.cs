using InnomateApp.Application.DTOs.Sales.Requests;
using InnomateApp.Application.DTOs.Sales.Responses;

namespace InnomateApp.Application.Interfaces.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleResponse>> GetAllAsync();
        Task<SaleResponse?> GetByIdAsync(int saleId);
        Task<SaleResponse> CreateAsync(CreateSaleRequest request);
        Task<SaleResponse?> UpdateAsync(UpdateSaleRequest request);
        Task<bool> DeleteAsync(int saleId);
        Task<string> GetNextInvoiceNumberAsync();
        Task<SaleResponse> AddPaymentAsync(AddPaymentRequest request);
    }
}
