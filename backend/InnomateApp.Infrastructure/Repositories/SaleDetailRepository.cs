using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    public class SaleDetailRepository : GenericRepository<SaleDetail>, ISaleDetailRepository
    {
        private readonly AppDbContext _context;

        public SaleDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns sale details for a given sale id, including Product navigation.
        /// Read-only (AsNoTracking) since caller likely only needs to view data.
        /// </summary>
        public async Task<IEnumerable<SaleDetail>> GetBySaleIdAsync(int saleId)
        {
            return await _context.SaleDetails
                .Where(sd => sd.SaleId == saleId)
                .Include(sd => sd.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Optional: get single SaleDetail by id with Product included.
        /// </summary>
        public async Task<SaleDetail?> GetByIdWithProductAsync(int id)
        {
            return await _context.SaleDetails
                .Where(sd => sd.SaleDetailId == id)
                .Include(sd => sd.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
