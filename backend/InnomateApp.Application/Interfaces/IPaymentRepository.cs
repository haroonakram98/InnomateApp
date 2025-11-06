using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Get all payments related to a specific Sale.
        /// </summary>
        Task<IEnumerable<Payment>> GetBySaleIdAsync(int saleId);

        /// <summary>
        /// Get total paid amount for a given Sale.
        /// </summary>
        Task<decimal> GetTotalPaidForSaleAsync(int saleId);
    }
}
