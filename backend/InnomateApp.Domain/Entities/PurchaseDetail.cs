using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class PurchaseDetail
    {
        [Key]
        public int PurchaseDetailId { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal RemainingQty { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public Purchase Purchase { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public void CalculateTotal()
        {
            TotalCost = Quantity * UnitCost;
        }

        public void UpdateRemainingQuantity(decimal soldQuantity)
        {
            if (soldQuantity > RemainingQty)
                throw new InvalidOperationException("Sold quantity exceeds remaining quantity");

            RemainingQty -= soldQuantity;
        }
    }
}
