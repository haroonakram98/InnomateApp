using InnomateApp.Domain.Common;
using InnomateApp.Domain.Events;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Purchase aggregate root with business logic
    /// </summary>
    public class Purchase : TenantEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        [Key]
        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime? ReceivedDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Received, Cancelled
        public string Notes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Supplier? Supplier { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        
        // Domain events
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Purchase() { }

        /// <summary>
        /// Factory method to create a new purchase
        /// </summary>
        public static Purchase Create(int tenantId, int supplierId, string invoiceNo, int createdBy, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceNo))
                throw new BusinessRuleViolationException("Invoice number is required");

            var purchase = new Purchase
            {
                SupplierId = supplierId,
                InvoiceNo = invoiceNo,
                CreatedBy = createdBy,
                Notes = notes ?? string.Empty,
                PurchaseDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            purchase.SetTenantId(tenantId);
            return purchase;
        }

        /// <summary>
        /// Calculate total amount from purchase details
        /// </summary>
        public void CalculateTotal()
        {
            if (PurchaseDetails == null || !PurchaseDetails.Any())
            {
                TotalAmount = 0;
                return;
            }

            TotalAmount = 0;
            foreach (var detail in PurchaseDetails)
            {
                detail.CalculateTotal();
                TotalAmount += detail.TotalCost;
            }
        }

        /// <summary>
        /// Mark purchase as received
        /// </summary>
        public void MarkAsReceived()
        {
            if (Status == "Cancelled")
                throw new BusinessRuleViolationException("Cannot receive a cancelled purchase");

            if (Status == "Received")
                throw new BusinessRuleViolationException("Purchase is already received");

            Status = "Received";
            ReceivedDate = DateTime.UtcNow;
            
            _domainEvents.Add(new PurchaseReceivedEvent(PurchaseId, TotalAmount));
        }

        /// <summary>
        /// Cancel the purchase
        /// </summary>
        public void Cancel(string reason)
        {
            if (Status == "Received")
                throw new BusinessRuleViolationException("Cannot cancel a received purchase");

            Status = "Cancelled";
            Notes = $"{Notes}\nCancelled: {reason}";
        }

        /// <summary>
        /// Validate purchase can be processed
        /// </summary>
        public void ValidateForProcessing()
        {
            if (PurchaseDetails == null || !PurchaseDetails.Any())
                throw new BusinessRuleViolationException("Purchase must have at least one item");

            if (Status == "Cancelled")
                throw new BusinessRuleViolationException("Cannot process a cancelled purchase");
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
