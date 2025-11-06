namespace InnomateApp.Application.DTOs.Sales.Requests
{
    public class CreateSaleDetailRequest
    {
        public int ProductId { get; set; }
        public int? PurchaseDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreatePaymentRequest
    {
        public string PaymentMethod { get; set; } = "Cash";
        public decimal Amount { get; set; }
        public string? ReferenceNo { get; set; }
    }

    public class CreateSaleRequest
    {
        public int CustomerId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public List<CreateSaleDetailRequest> SaleDetails { get; set; } = new();
        public List<CreatePaymentRequest> Payments { get; set; } = new();
    }
}
