using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Application.DTOs.Sales.Requests
{
    public class CreateSaleDetailRequest
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; } = 0;
        public string DiscountType { get; set; } = "Amount"; // "Amount" or "Percentage"
        public decimal DiscountPercentage { get; set; } = 0;
    }

    public class CreatePaymentRequest
    {
        [Required]
        public string PaymentMethod { get; set; } = "Cash";
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        public string? ReferenceNo { get; set; }
    }

    public class CreateSaleRequest
    {
        public int? CustomerId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal BalanceAmount { get; set; } = 0;
        public bool IsFullyPaid { get; set; } = false;
        public decimal Discount { get; set; } = 0;
        public string DiscountType { get; set; } = "Amount"; // "Amount" or "Percentage"
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal SubTotal { get; set; } = 0; // Total before discount
        public List<CreateSaleDetailRequest> SaleDetails { get; set; } = new();
        public List<CreatePaymentRequest> Payments { get; set; } = new();
    }
}
