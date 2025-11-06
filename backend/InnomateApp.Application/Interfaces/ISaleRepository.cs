using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces.Repositories
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        Task<Sale?> GetByIdWithDetailsAsync(int saleId);
        Task<IEnumerable<Sale>> GetAllWithDetailsAsync();
    }
}
