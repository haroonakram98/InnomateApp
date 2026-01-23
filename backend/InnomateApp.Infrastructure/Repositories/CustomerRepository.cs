using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> ExistsAsync(string name, string phone)
        {
            return await _context.Customers.AnyAsync(c => 
                c.Name.Trim().ToLower() == name.Trim().ToLower() || 
                (!string.IsNullOrEmpty(phone) && c.Phone == phone));
        }

        public async Task<int> CountAsync()
        {
             return await _context.Customers.CountAsync();
        }

        public async Task<int> CountAsync(DateTime startDate, DateTime endDate)
        {
            // Adjust endDate to include the full day
            var actualEndDate = endDate.Date.AddDays(1).AddTicks(-1);
            return await _context.Customers.CountAsync(c => c.CreatedAt >= startDate.Date && c.CreatedAt <= actualEndDate);
        }
    }
}
