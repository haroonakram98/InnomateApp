using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces.Repositories
{
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
        Task<Tenant?> GetByCodeAsync(string code);
    }
}
