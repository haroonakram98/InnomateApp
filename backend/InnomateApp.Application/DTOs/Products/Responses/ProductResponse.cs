namespace InnomateApp.Application.DTOs.Products.Responses
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? SKU { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        public decimal DefaultSalePrice { get; set; }
        
        // Basic stock info if available
        public decimal CurrentStock { get; set; }
        public string StockStatus { get; set; } = "Unknown";
        public decimal TotalValue { get; set; }
    }

    public class ProductLookupResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public decimal DefaultSalePrice { get; set; }
    }
}
