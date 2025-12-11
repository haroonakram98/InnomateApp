using InnomateApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces.Repositories
{
    public interface IReturnRepository : IGenericRepository<Return>
    {
        Task<IEnumerable<Return>> GetReturnsBySaleIdAsync(int saleId);
        Task<Return?> GetReturnWithDetailsAsync(int returnId);
    }
}
