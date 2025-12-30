using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class ReturnDetail : TenantEntity
    {
        [Key]
        public int ReturnDetailId { get; set; }
        public int ReturnId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public Return Return { get; set; } = null!;
        public Product Product { get; set; } = null!;

        public ReturnDetail() { }

        public void CalculateTotal()
        {
            Total = Quantity * UnitPrice;
        }

        public static ReturnDetail Create(int tenantId, int productId, decimal quantity, decimal unitPrice)
        {
            var detail = new ReturnDetail
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Total = quantity * unitPrice
            };
            detail.SetTenantId(tenantId);
            return detail;
        }
    }
}
