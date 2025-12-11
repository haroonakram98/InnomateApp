using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class StockTransaction
    {
        [Key]
        public int StockTransactionId { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public char TransactionType { get; set; } // 'P', 'S', 'A'
        public int ReferenceId { get; set; }
        // +Quantity = Purchase, -Quantity = Sale
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Product? Product { get; set; } = null!;

        public decimal TotalCost { get; set; }
        public string Reference { get; set; } // PurchaseId, SaleId, etc.
        public string Notes { get; set; } = string.Empty;
    }
}
