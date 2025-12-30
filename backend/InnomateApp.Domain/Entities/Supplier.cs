using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class Supplier : TenantEntity
    {
        public int SupplierId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(200), EmailAddress]
        public string Email { get; set; }

        [StringLength(20), Required]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        public string Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        public Supplier() { }

        public static Supplier Create(int tenantId, string name, string email, string phone, string address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Supplier name is required");
            
            if (string.IsNullOrWhiteSpace(phone))
                throw new BusinessRuleViolationException("Supplier phone is required");

            var supplier = new Supplier
            {
                Name = name.Trim(),
                Email = email?.Trim(),
                Phone = phone?.Trim(),
                Address = address?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            supplier.SetTenantId(tenantId);
            return supplier;
        }

        public void Update(string name, string email, string phone, string address = null, string contactPerson = null, string notes = null)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            ContactPerson = contactPerson;
            Notes = notes;
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

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }
    }
}