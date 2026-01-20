using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Represents a business/customer (Tenant) in the multi-tenant system.
    /// </summary>
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty; // Unique code e.g. "ABC-CORP"

        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? SubscriptionExpiry { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
        
        public static Tenant Create(string name, string code)
        {
            return new Tenant
            {
                Name = name,
                Code = code.ToUpperInvariant(),
                IsActive = true,
                CreatedAt = DateTime.Now
            };
        }
    }
}
