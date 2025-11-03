using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal DefaultSalePrice { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        public decimal? StockBalance { get; set; }

        // Add these(from StockSummary)
        public decimal AverageCost { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? SKU { get; set; }
        public decimal DefaultSalePrice { get; set; }
        public int ReorderLevel { get; set; } = 0;
    }

    public class UpdateProductDto : CreateProductDto
    {
        public int ProductId { get; set; }
    }
}
