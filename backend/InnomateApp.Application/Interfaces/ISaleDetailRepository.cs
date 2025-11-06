using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces.Repositories
{
    public interface ISaleDetailRepository : IGenericRepository<SaleDetail>
    {
        /// <summary>
        /// Get all sale details for a given SaleId, including related Product.
        /// </summary>
        Task<IEnumerable<SaleDetail>> GetBySaleIdAsync(int saleId);

        /// <summary>
        /// Get single SaleDetail with Product navigation by its id.
        /// </summary>
        Task<SaleDetail?> GetByIdWithProductAsync(int saleDetailId);
    }
}
