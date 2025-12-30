using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class SaleDetail : TenantEntity
    {
        [Key]
        public int SaleDetailId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        
        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = "Amount";
        public decimal DiscountPercentage { get; set; }
        public decimal NetAmount { get; set; }

        public int? PurchaseDetailId { get; set; }

        public Sale Sale { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public PurchaseDetail? PurchaseDetail { get; set; }
        
        public virtual ICollection<SaleDetailBatch> UsedBatches { get; set; } = new List<SaleDetailBatch>();
        
        public SaleDetail() { }

        public void CalculateTotal()
        {
            Total = Quantity * UnitPrice;
            TotalCost = Quantity * UnitCost;
            
            if (DiscountType == "Percentage")
            {
                DiscountPercentage = Discount;
                Discount = Total * (DiscountPercentage / 100);
            }
            else
            {
                if (Total > 0)
                    DiscountPercentage = (Discount / Total) * 100;
            }

            NetAmount = Total - Discount;
            Profit = NetAmount - TotalCost;
        }

        public static SaleDetail Create(int tenantId, int productId, decimal quantity, decimal unitPrice, decimal unitCost, decimal discount = 0, string discountType = "Amount")
        {
            var detail = new SaleDetail
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                UnitCost = unitCost,
                Discount = discount,
                DiscountType = discountType
            };
            detail.CalculateTotal();
            detail.SetTenantId(tenantId);
            return detail;
        }
    }
}
