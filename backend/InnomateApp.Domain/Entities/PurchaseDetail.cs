using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class PurchaseDetail : TenantEntity
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
        
        // Navigation for FIFO
        public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

        public PurchaseDetail() { }

        public void CalculateTotal()
        {
            TotalCost = Quantity * UnitCost;
        }

        public static PurchaseDetail Create(int tenantId, int productId, decimal quantity, decimal unitCost, string? batchNo = null, DateTime? expiryDate = null)
        {
            var detail = new PurchaseDetail
            {
                ProductId = productId,
                Quantity = quantity,
                RemainingQty = quantity,
                UnitCost = unitCost,
                TotalCost = quantity * unitCost,
                BatchNo = batchNo,
                ExpiryDate = expiryDate
            };
            detail.SetTenantId(tenantId);
            return detail;
        }
    }
}
