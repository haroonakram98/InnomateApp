namespace InnomateApp.Application.DTOs.Sales.Requests
{
    public class UpdateSaleRequest
    {
        public int SaleId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public List<CreateSaleDetailRequest> SaleDetails { get; set; } = new();
        public List<CreatePaymentRequest> Payments { get; set; } = new();
    }
}
