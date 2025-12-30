using System.Threading.Tasks;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(AppDbContext context) : base(context) { }

        public async Task<Tenant?> GetByCodeAsync(string code)
        {
            return await _context.Tenants.FirstOrDefaultAsync(t => t.Code == code.ToUpperInvariant());
        }
    }
}
