using InnomateApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IFifoService
    {
        Task<FifoAllocationResult> AllocateStockForSaleAsync(int productId, decimal quantity);
        Task<decimal> ReturnStockFromSaleAsync(int saleDetailId, decimal quantity);
        Task<decimal> CalculateFifoCostAsync(int productId, decimal quantity);
    }
}
