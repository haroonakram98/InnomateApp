using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class SaleDetailBatch
    {
        public int SaleDetailBatchId { get; set; }
        public int SaleDetailId { get; set; }
        public int PurchaseDetailId { get; set; }  // Which batch was used
        public decimal QuantityUsed { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }

        public SaleDetail SaleDetail { get; set; } = null!;
        public PurchaseDetail PurchaseDetail { get; set; } = null!;
    }

}
