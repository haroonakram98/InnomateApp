using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Common
{
    public class FifoAllocationResult
    {
        public bool Success { get; set; }
        public decimal TotalCost { get; set; }
        public List<FifoAllocationDetail> Allocations { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class FifoAllocationDetail
    {
        public int PurchaseDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
