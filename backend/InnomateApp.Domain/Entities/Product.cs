using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        [MaxLength(50)]
        public string? SKU { get; set; }
        public int ReorderLevel { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        [Required]
        public decimal DefaultSalePrice { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
        public StockSummary? StockSummary { get; set; }
    }
}
