using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int? CustomerId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal PaidAmount { get; set; } = 0;
        public decimal BalanceAmount { get; set; } = 0;
        public bool IsFullyPaid { get; set; } = false;
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Customer? Customer { get; set; }
        // New discount fields
        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = "Amount"; // "Amount" or "Percentage"
        public decimal DiscountPercentage { get; set; }
        public decimal SubTotal { get; set; } // Total before discount
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
