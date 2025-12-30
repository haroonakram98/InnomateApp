using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class Category : TenantEntity
    {
        [Key]
        public int CategoryId { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public Category() { }

        public static Category Create(int tenantId, string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Category name is required");

            var category = new Category
            {
                Name = name.Trim(),
                Description = description
            };

            category.SetTenantId(tenantId);
            return category;
        }
    }
}
