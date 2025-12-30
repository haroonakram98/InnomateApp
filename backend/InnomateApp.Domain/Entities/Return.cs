using InnomateApp.Domain.Common;
using InnomateApp.Domain.Events;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Return aggregate root with business logic
    /// </summary>
    public class Return : TenantEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public int ReturnId { get; set; }
        public int SaleId { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalRefund { get; set; }
        public string? Reason { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Sale? Sale { get; set; }
        public ICollection<ReturnDetail>? ReturnDetails { get; set; }
        
        // Domain events
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Return() { }

        /// <summary>
        /// Factory method to create a new return
        /// </summary>
        public static Return Create(int tenantId, int saleId, int createdBy, string? reason = null)
        {
            var returnEntity = new Return
            {
                SaleId = saleId,
                CreatedBy = createdBy,
                Reason = reason,
                ReturnDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            returnEntity.SetTenantId(tenantId);
            return returnEntity;
        }

        /// <summary>
        /// Calculate total refund from return details
        /// </summary>
        public void CalculateTotalRefund()
        {
            if (ReturnDetails == null || !ReturnDetails.Any())
            {
                TotalRefund = 0;
                return;
            }

            TotalRefund = ReturnDetails.Sum(d => d.Total);
        }

        /// <summary>
        /// Complete the return and raise event
        /// </summary>
        public void Complete()
        {
            if (ReturnDetails == null || !ReturnDetails.Any())
                throw new BusinessRuleViolationException("Return must have at least one item");

            CalculateTotalRefund();
            _domainEvents.Add(new ReturnCompletedEvent(ReturnId, SaleId, TotalRefund));
        }

        /// <summary>
        /// Validate return can be processed
        /// </summary>
        public void ValidateForProcessing()
        {
            if (ReturnDetails == null || !ReturnDetails.Any())
                throw new BusinessRuleViolationException("Return must have at least one item");

            if (TotalRefund < 0)
                throw new BusinessRuleViolationException("Total refund cannot be negative");
        }

        /// <summary>
        /// Clear domain events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
