namespace InnomateApp.Application.DTOs;

public class SaleDto
{
    public int SaleId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    public List<SaleDetailDto> SaleDetails { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class SaleDetailDto
{
    public int SaleDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class PaymentDto
{
    public int PaymentId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? ReferenceNo { get; set; }
}
