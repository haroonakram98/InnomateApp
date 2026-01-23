using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(string name, string phone);
        Task<int> CountAsync();
        Task<int> CountAsync(DateTime startDate, DateTime endDate);
    }
}
