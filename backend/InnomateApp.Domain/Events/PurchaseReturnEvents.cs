using InnomateApp.Domain.Common;

namespace InnomateApp.Domain.Events
{
    /// <summary>
    /// Raised when a purchase is received
    /// </summary>
    public class PurchaseReceivedEvent : IDomainEvent
    {
        public int PurchaseId { get; }
        public decimal TotalAmount { get; }
        public DateTime OccurredOn { get; }

        public PurchaseReceivedEvent(int purchaseId, decimal totalAmount)
        {
            PurchaseId = purchaseId;
            TotalAmount = totalAmount;
            OccurredOn = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Raised when a return is completed
    /// </summary>
    public class ReturnCompletedEvent : IDomainEvent
    {
        public int ReturnId { get; }
        public int SaleId { get; }
        public decimal TotalRefund { get; }
        public DateTime OccurredOn { get; }

        public ReturnCompletedEvent(int returnId, int saleId, decimal totalRefund)
        {
            ReturnId = returnId;
            SaleId = saleId;
            TotalRefund = totalRefund;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
