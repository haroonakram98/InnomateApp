using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public int CustomerId { get; set; }
        [MaxLength(50)]
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Customer? Customer { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
