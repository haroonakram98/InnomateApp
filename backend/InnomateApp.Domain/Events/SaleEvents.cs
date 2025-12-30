using InnomateApp.Domain.Common;
using InnomateApp.Domain.ValueObjects;

namespace InnomateApp.Domain.Events
{
    /// <summary>
    /// Raised when a sale is completed
    /// </summary>
    public class SaleCompletedEvent : IDomainEvent
    {
        public int SaleId { get; }
        public Money TotalAmount { get; }
        public DateTime OccurredOn { get; }

        public SaleCompletedEvent(int saleId, Money totalAmount)
        {
            SaleId = saleId;
            TotalAmount = totalAmount;
            OccurredOn = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Raised when stock is deducted for a sale
    /// </summary>
    public class StockDeductedEvent : IDomainEvent
    {
        public int ProductId { get; }
        public decimal Quantity { get; }
        public int SaleId { get; }
        public DateTime OccurredOn { get; }

        public StockDeductedEvent(int productId, decimal quantity, int saleId)
        {
            ProductId = productId;
            Quantity = quantity;
            SaleId = saleId;
            OccurredOn = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Raised when a payment is received for a sale
    /// </summary>
    public class PaymentReceivedEvent : IDomainEvent
    {
        public int SaleId { get; }
        public Money Amount { get; }
        public string PaymentMethod { get; }
        public DateTime OccurredOn { get; }

        public PaymentReceivedEvent(int saleId, Money amount, string paymentMethod)
        {
            SaleId = saleId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
