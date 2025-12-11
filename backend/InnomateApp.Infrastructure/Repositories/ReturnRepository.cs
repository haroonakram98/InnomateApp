using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Data;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Repositories
{
    public class ReturnRepository : GenericRepository<Return>, IReturnRepository
    {
        public ReturnRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Return>> GetReturnsBySaleIdAsync(int saleId)
        {
            return await _context.Returns
                .Include(r => r.ReturnDetails)
                .ThenInclude(rd => rd.Product)
                .Where(r => r.SaleId == saleId)
                .ToListAsync();
        }

        public async Task<Return?> GetReturnWithDetailsAsync(int returnId)
        {
            return await _context.Returns
                .Include(r => r.ReturnDetails)
                .ThenInclude(rd => rd.Product)
                .Include(r => r.Sale)
                .FirstOrDefaultAsync(r => r.ReturnId == returnId);
        }
    }
}
