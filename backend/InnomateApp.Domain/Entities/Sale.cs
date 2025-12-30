using InnomateApp.Domain.Common;
using InnomateApp.Domain.Events;
using InnomateApp.Domain.ValueObjects;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Sale aggregate root - Rich domain model with business logic
    /// Public setters for EF Core/AutoMapper/cross-assembly compatibility
    /// Use business methods for proper encapsulation
    /// </summary>
    public class Sale : TenantEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        // Properties with public setters (cross-assembly compatible)
        public int SaleId { get; set; }
        public int? CustomerId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool IsFullyPaid { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        
        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = "Amount";
        public decimal DiscountPercentage { get; set; }
        public decimal SubTotal { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Customer? Customer { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        
        // Domain events (read-only externally)
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        // Parameterless constructor for EF Core
        public Sale() { }

        /// <summary>
        /// Factory method to create a new sale with validation
        /// </summary>
        public static Sale Create(int tenantId, int? customerId, string invoiceNo, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(invoiceNo))
                throw new BusinessRuleViolationException("Invoice number is required");

            var sale = new Sale
            {
                InvoiceNo = invoiceNo,
                CustomerId = customerId,
                CreatedBy = createdBy,
                SaleDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            sale.SetTenantId(tenantId);
            return sale;
        }

        /// <summary>
        /// Apply discount to the sale
        /// </summary>
        public void ApplyDiscount(decimal discount, string discountType = "Amount")
        {
            if (discount < 0)
                throw new BusinessRuleViolationException("Discount cannot be negative");

            if (discountType == "Percentage" && discount > 100)
                throw new BusinessRuleViolationException("Percentage discount cannot exceed 100%");

            Discount = discount;
            DiscountType = discountType;

            if (discountType == "Percentage")
            {
                DiscountPercentage = discount;
                Discount = SubTotal * (discount / 100);
            }

            RecalculateTotals();
        }

        /// <summary>
        /// Record a payment
        /// </summary>
        public void RecordPayment(decimal amount, string paymentMethod)
        {
            if (amount <= 0)
                throw new BusinessRuleViolationException("Payment amount must be greater than zero");

            if (amount > BalanceAmount)
                throw new BusinessRuleViolationException($"Payment amount ({amount}) exceeds balance ({BalanceAmount})");

            PaidAmount += amount;
            BalanceAmount = TotalAmount - PaidAmount;
            IsFullyPaid = BalanceAmount == 0;
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new PaymentReceivedEvent(SaleId, new Money(amount), paymentMethod));
        }

        /// <summary>
        /// Mark sale as completed and raise domain event
        /// </summary>
        public void Complete()
        {
            if (SaleDetails == null || !SaleDetails.Any())
                throw new BusinessRuleViolationException("Cannot complete sale without items");

            RecalculateTotals();
            _domainEvents.Add(new SaleCompletedEvent(SaleId, new Money(TotalAmount)));
        }

        /// <summary>
        /// Soft delete the sale
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Clear domain events (call after dispatching)
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Recalculate all totals based on details
        /// </summary>
        public void RecalculateTotals()
        {
            if (SaleDetails != null && SaleDetails.Any())
            {
                SubTotal = SaleDetails.Sum(d => d.Total);
                TotalAmount = SubTotal - Discount;
                TotalCost = SaleDetails.Sum(d => d.Quantity * d.UnitCost);
                TotalProfit = TotalAmount - TotalCost;
                ProfitMargin = TotalAmount > 0 ? (TotalProfit / TotalAmount) * 100 : 0;
                BalanceAmount = TotalAmount - PaidAmount;
                IsFullyPaid = BalanceAmount == 0;
            }
        }
    }
}
