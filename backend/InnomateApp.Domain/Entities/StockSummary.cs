using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class StockSummary : TenantEntity
    {
        [Key]
        public int StockSummaryId { get; set; }
        public int ProductId { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Balance { get; set; }
        public decimal AverageCost { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastUpdated { get; set; }

        public Product Product { get; set; } = null!;

        public StockSummary() { }

        public static StockSummary Create(int tenantId, int productId)
        {
            var summary = new StockSummary
            {
                ProductId = productId,
                LastUpdated = DateTime.UtcNow
            };

            summary.SetTenantId(tenantId);
            return summary;
        }

        public void UpdateStock(decimal quantityChange, decimal unitCost, bool isAddition)
        {
            if (isAddition)
            {
                TotalIn += quantityChange;
                // Simple weighted average cost calculation
                if (Balance + quantityChange > 0)
                {
                   AverageCost = ((Balance * AverageCost) + (quantityChange * unitCost)) / (Balance + quantityChange);
                }
            }
            else
            {
                TotalOut += quantityChange;
            }

            Balance = TotalIn - TotalOut;
            TotalValue = Balance * AverageCost;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
