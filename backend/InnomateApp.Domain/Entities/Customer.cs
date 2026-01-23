using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Customer entity with centralized business logic (Single Source of Truth)
    /// </summary>
    public class Customer : TenantEntity
    {
        [Key]
        public int CustomerId { get; set; }
        
        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(200)]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

        public Customer() { }

        /// <summary>
        /// Domain Factory Method - Centralized rules for creating a customer.
        /// </summary>
        public static Customer Create(int tenantId, string name, string? phone = null, string? email = null, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Domain Rule: Customer name is required");

            var customer = new Customer
            {
                Name = name.Trim(),
                Phone = phone?.Trim(),
                Email = email?.Trim()?.ToLower(),
                Address = address?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            customer.SetTenantId(tenantId);
            return customer;
        }

        /// <summary>
        /// Update customer details with validation
        /// </summary>
        public void Update(string name, string? phone = null, string? email = null, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Domain Rule: Customer name is required");

            Name = name.Trim();
            Phone = phone?.Trim();
            Email = email?.Trim()?.ToLower();
            Address = address?.Trim();
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

        public void ValidateForSale()
        {
            if (!IsActive)
                throw new BusinessRuleViolationException($"Customer {Name} is inactive and cannot perform purchases.");
        }
    }
}
