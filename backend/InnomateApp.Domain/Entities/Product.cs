using InnomateApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnomateApp.Domain.Entities
{
    /// <summary>
    /// Product entity with business logic
    /// Public setters for cross-assembly compatibility
    /// Use business methods for proper encapsulation
    /// </summary>
    public class Product : TenantEntity
    {
        [Key]
        public int ProductId { get; set; }
        
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        
        [MaxLength(50)]
        public string? SKU { get; set; }
        
        public int ReorderLevel { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal DefaultSalePrice { get; set; }

        // Navigation properties
        public Category Category { get; set; } = null!;
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
        public StockSummary? StockSummary { get; set; }

        // Parameterless constructor for EF Core
        public Product() { }

        /// <summary>
        /// Factory method to create a new product with validation
        /// </summary>
        public static Product Create(int tenantId, string name, int categoryId, string? sku, 
            decimal defaultSalePrice, int reorderLevel = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Product name is required");

            if (defaultSalePrice < 0)
                throw new BusinessRuleViolationException("Default sale price cannot be negative");

            if (reorderLevel < 0)
                throw new BusinessRuleViolationException("Reorder level cannot be negative");

            var product = new Product
            {
                Name = name.Trim(),
                CategoryId = categoryId,
                SKU = sku?.Trim(),
                DefaultSalePrice = defaultSalePrice,
                ReorderLevel = reorderLevel,
                IsActive = true
            };

            product.SetTenantId(tenantId);
            return product;
        }

        /// <summary>
        /// Update product details
        /// </summary>
        public void Update(string name, int categoryId, string? sku, decimal defaultSalePrice, int reorderLevel)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Product name is required");

            if (defaultSalePrice < 0)
                throw new BusinessRuleViolationException("Default sale price cannot be negative");

            Name = name.Trim();
            CategoryId = categoryId;
            SKU = sku?.Trim();
            DefaultSalePrice = defaultSalePrice;
            ReorderLevel = reorderLevel;
        }

        /// <summary>
        /// Deactivate the product
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// Activate the product
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        /// <summary>
        /// Check if product is below reorder level
        /// </summary>
        public bool IsLowStock(decimal currentStock)
        {
            return currentStock <= ReorderLevel;
        }

        /// <summary>
        /// Validate if product can be sold
        /// </summary>
        public void ValidateForSale()
        {
            if (!IsActive)
                throw new InactiveEntityException(nameof(Product), ProductId);
        }
    }
}
