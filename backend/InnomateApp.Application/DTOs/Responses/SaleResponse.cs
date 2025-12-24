namespace InnomateApp.Application.DTOs.Sales.Responses
{
    public class SaleDetailResponse
    {
        public int SaleDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }

        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = "Amount";
        public decimal DiscountPercentage { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Total { get; set; }
        public List<FIFOLayerDto> UsedBatches { get; set; } = new();
    }

    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNo { get; set; }
    }

    public class CustomerShortResponse
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }

    public class SaleResponse
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public int? CustomerId { get; set; }

        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool IsFullyPaid { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = "Amount";
        public decimal DiscountPercentage { get; set; }
        public decimal SubTotal { get; set; }

        public CustomerShortResponse? Customer { get; set; }
        public List<SaleDetailResponse> SaleDetails { get; set; } = new();
        public List<PaymentResponse> Payments { get; set; } = new();
    }
}
