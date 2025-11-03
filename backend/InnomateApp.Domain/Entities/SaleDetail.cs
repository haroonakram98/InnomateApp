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
        [Key]
        public int SaleDetailId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int? PurchaseDetailId { get; set; } // FIFO link
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public Sale Sale { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public PurchaseDetail? PurchaseDetail { get; set; }
        [NotMapped]
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
