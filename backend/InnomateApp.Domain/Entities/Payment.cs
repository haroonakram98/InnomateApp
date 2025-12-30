using InnomateApp.Domain.Common;

namespace InnomateApp.Domain.Entities
{
    public class Payment : TenantEntity
    {
        public int PaymentId { get; set; }
        public int SaleId { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; 
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNo { get; set; } 

        public Sale? Sale { get; set; }

        public Payment() { }

        public static Payment Create(int tenantId, int saleId, decimal amount, string paymentMethod, string? referenceNo = null)
        {
            if (amount <= 0)
                throw new BusinessRuleViolationException("Payment amount must be greater than zero");

            var payment = new Payment
            {
                SaleId = saleId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                ReferenceNo = referenceNo,
                PaymentDate = DateTime.UtcNow
            };

            payment.SetTenantId(tenantId);
            return payment;
        }
    }
}
