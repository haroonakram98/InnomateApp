using InnomateApp.Domain.Common;

namespace InnomateApp.Domain.Entities
{
    public class SaleDetailBatch : TenantEntity
    {
        public int SaleDetailBatchId { get; set; }
        public int SaleDetailId { get; set; }
        public int PurchaseDetailId { get; set; } 
        public decimal QuantityUsed { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public SaleDetail SaleDetail { get; set; } = null!;
        public PurchaseDetail PurchaseDetail { get; set; } = null!;

        public SaleDetailBatch() { }

        public static SaleDetailBatch Create(int tenantId, int saleDetailId, int purchaseDetailId, decimal quantityUsed, decimal unitCost)
        {
            var batch = new SaleDetailBatch
            {
                SaleDetailId = saleDetailId,
                PurchaseDetailId = purchaseDetailId,
                QuantityUsed = quantityUsed,
                UnitCost = unitCost,
                TotalCost = quantityUsed * unitCost,
                CreatedAt = DateTime.Now
            };
            batch.SetTenantId(tenantId);
            return batch;
        }
    }
}
