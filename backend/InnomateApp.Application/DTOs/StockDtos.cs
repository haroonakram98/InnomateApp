using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.DTOs
{
    public class StockSummaryDto
    {
        public int StockSummaryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal AverageCost { get; set; }
        public decimal TotalValue { get; set; }

        public decimal StockQuanitity { get; set; }
    }

    public class StockTransactionDto
    {
        public int StockTransactionId { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalCost { get; set; }
        public decimal UnitCost { get; set; }
        public string TransactionTypeName => TransactionType switch
        {
            "I" => "Purchase",
            "O" => "Sale",
            "R" => "Adjustment",
            _ => "Unknown"
        };
    }

    public class FifoBatchDto
    {
        public int PurchaseDetailId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class StockMovementDto
    {
        public int ProductId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // "P", "S", "A"
        public int ReferenceId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }

    public class FIFOSaleResultDto
    {
        public int ProductId { get; set; }
        public decimal QuantitySold { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageCost { get; set; }
        public List<FIFOLayerDto> LayersUsed { get; set; } = new();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class FIFOLayerDto
    {
        public int PurchaseDetailId { get; set; }
        public decimal QuantityUsed { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
    public class StockValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public Dictionary<int, decimal> AvailableStock { get; set; } = new();
    }
}
