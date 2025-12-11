using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? SKU { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        public decimal DefaultSalePrice { get; set; }
        public StockSummaryDto? StockSummary { get; set; }
    }

    public class ProductStockDto : ProductDto
    {
        public decimal AvailableStock => StockSummary?.Balance ?? 0;
        public decimal AvailableQuantity => (StockSummary?.TotalIn - StockSummary?.TotalOut) ?? 0;
        public string StockStatus => AvailableStock <= ReorderLevel ? "Low Stock" : "In Stock";
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(0.01, double.MaxValue)]
        public decimal DefaultSalePrice { get; set; }
    }

    public class UpdateProductDto : CreateProductDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}
