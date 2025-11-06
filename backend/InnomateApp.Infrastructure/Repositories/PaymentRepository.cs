using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnomateApp.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all payments for a specific sale (read-only).
        /// </summary>
        public async Task<IEnumerable<Payment>> GetBySaleIdAsync(int saleId)
        {
            return await _context.Payments
                .Where(p => p.SaleId == saleId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Optional: get total paid amount for a sale.
        /// Useful for calculating outstanding balance.
        /// </summary>
        public async Task<decimal> GetTotalPaidForSaleAsync(int saleId)
        {
            return await _context.Payments
                .Where(p => p.SaleId == saleId)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
        }
    }
}
