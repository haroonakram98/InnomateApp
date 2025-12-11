using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public int SupplierId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime? ReceivedDate { get; set; }
        public string Status { get; set; } = string.Empty;// Pending, Received, Cancelled
        public string Notes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Supplier? Supplier { get; set; } = null!;
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

        public void CalculateTotal()
        {
            TotalAmount = 0;
            foreach (var detail in PurchaseDetails)
            {
                detail.CalculateTotal();
                TotalAmount += detail.TotalCost;
            }
        }

        public void MarkAsReceived()
        {
            Status = "Received";
            ReceivedDate = DateTime.Now;
        }
    }
}
