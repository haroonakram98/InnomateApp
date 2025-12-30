using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Customer entity with business logic
    /// </summary>
    public class Customer : TenantEntity
    {
        [Key]
        public int CustomerId { get; set; }
        
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(200)]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();

        public Customer() { }

        /// <summary>
        /// Factory method to create a new customer
        /// </summary>
        public static Customer Create(int tenantId, string name, string? phone = null, string? email = null, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Customer name is required");

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                throw new BusinessRuleViolationException("Invalid email format");

            var customer = new Customer
            {
                Name = name.Trim(),
                Phone = phone?.Trim(),
                Email = email?.Trim(),
                Address = address?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            customer.SetTenantId(tenantId);
            return customer;
        }

        /// <summary>
        /// Update customer details
        /// </summary>
        public void Update(string name, string? phone = null, string? email = null, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Customer name is required");

            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                throw new BusinessRuleViolationException("Invalid email format");

            Name = name.Trim();
            Phone = phone?.Trim();
            Email = email?.Trim();
            Address = address?.Trim();
        }

        /// <summary>
        /// Deactivate the customer
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Activate the customer
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Validate customer can make purchases
        /// </summary>
        public void ValidateForSale()
        {
            if (!IsActive)
                throw new InactiveEntityException(nameof(Customer), CustomerId);
        }

        /// <summary>
        /// Simple email validation
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
