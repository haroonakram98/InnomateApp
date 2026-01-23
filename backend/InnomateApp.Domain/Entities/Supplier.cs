using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class Supplier : TenantEntity
    {
        public int SupplierId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [Required, StringLength(200)]
        public string Email { get; set; } = null!;

        [Required, StringLength(20)]
        public string Phone { get; set; } = null!;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        public Supplier() { }

        /// <summary>
        /// Domain Factory Method - Centralized rules for creating a supplier.
        /// </summary>
        public static Supplier Create(int tenantId, string name, string email, string phone, string? address = null)
        {
            // Business Rule Validation (The "Last Line of Defense")
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Domain Rule: Supplier name is required");
            
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleViolationException("Domain Rule: Supplier email is required");

            if (string.IsNullOrWhiteSpace(phone))
                throw new BusinessRuleViolationException("Domain Rule: Supplier phone is required");

            var supplier = new Supplier
            {
                Name = name.Trim(),
                Email = email.Trim().ToLower(),
                Phone = phone.Trim(),
                Address = address?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            supplier.SetTenantId(tenantId);
            return supplier;
        }

        public void Update(string name, string email, string phone, string? address = null, string? contactPerson = null, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleViolationException("Name cannot be empty");
            if (string.IsNullOrWhiteSpace(email)) throw new BusinessRuleViolationException("Email cannot be empty");
            if (string.IsNullOrWhiteSpace(phone)) throw new BusinessRuleViolationException("Phone cannot be empty");

            Name = name.Trim();
            Email = email.Trim().ToLower();
            Phone = phone.Trim();
            Address = address?.Trim();
            ContactPerson = contactPerson?.Trim();
            Notes = notes?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleStatus()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
        }

        // Deprecated: Logic moved to Create/Update for "Fail-Fast" behavior
        public bool IsValid() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Phone) && !string.IsNullOrWhiteSpace(Email);
    }
}