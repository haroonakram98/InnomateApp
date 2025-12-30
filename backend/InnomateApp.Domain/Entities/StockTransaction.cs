using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class StockTransaction : TenantEntity
    {
        [Key]
        public int StockTransactionId { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public char TransactionType { get; set; } // 'P', 'S', 'A'
        public int ReferenceId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalCost { get; set; }
        public string Reference { get; set; } 
        public string Notes { get; set; } = string.Empty;

        public Product? Product { get; set; }

        public StockTransaction() { }

        public static StockTransaction Create(int tenantId, int productId, char type, int refId, decimal qty, decimal cost, string reference, string? notes = null)
        {
            var transaction = new StockTransaction
            {
                ProductId = productId,
                TransactionType = type,
                ReferenceId = refId,
                Quantity = qty,
                UnitCost = cost,
                TotalCost = qty * cost,
                Reference = reference,
                Notes = notes ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            transaction.SetTenantId(tenantId);
            return transaction;
        }
    }
}
