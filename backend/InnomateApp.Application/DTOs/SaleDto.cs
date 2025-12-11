using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Application.DTOs;

public class SaleDto
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CustomerId { get; set; }
    public decimal BalanceAmount { get; set; }
    public bool IsFullyPaid { get; set; }
    public decimal PaidAmount { get; set; }
    public List<SaleDetailDto> SaleDetails { get; set; } = new();
}

public class SaleDetailDto
{
    public int SaleDetailId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? PurchaseDetailId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public decimal CostPrice { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class CreateSaleDto
{
    [Required]
    public DateTime SaleDate { get; set; } = DateTime.Now;

    [Required]
    public string InvoiceNo { get; set; } = string.Empty;

    [Required]
    public int CreatedBy { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSaleDetailDto> SaleDetails { get; set; } = new();
}

public class CreateSaleDetailDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

public class SaleCalculationResultDto
{
    public decimal TotalAmount { get; set; }
    public decimal TotalCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal ProfitMargin { get; set; }

    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<SaleDetailCostDto> DetailCosts { get; set; } = new();
}

public class SaleDetailCostDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Profit { get; set; }
}