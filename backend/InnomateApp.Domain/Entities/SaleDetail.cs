using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class SaleDetail
    {
        public int SaleDetailId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal UnitCost { get; set; }      // Average cost from FIFO batches
        public decimal TotalCost { get; set; }     // Total cost (Quantity * UnitCost)
        public decimal Profit { get; set; }        // Total - TotalCost
        public int? PurchaseDetailId { get; set; }
        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = "Amount"; // "Amount" or "Percentage"
        public decimal DiscountPercentage { get; set; }
        public decimal NetAmount { get; set; } // Total after discount
        public Sale Sale { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public PurchaseDetail? PurchaseDetail { get; set; }
        public List<SaleDetailBatch> UsedBatches { get; set; } = new();
    }
}
