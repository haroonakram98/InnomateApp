using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        private readonly AppDbContext _context;

        public SaleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetAllWithDetailsAsync()
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.Payments)
                .OrderByDescending(s => s.SaleDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sale?> GetByIdWithDetailsAsync(int saleId)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.SaleId == saleId);
        }
    }
}
