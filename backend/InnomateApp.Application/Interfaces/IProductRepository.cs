using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetAllWithStockAsync();
    }
}
