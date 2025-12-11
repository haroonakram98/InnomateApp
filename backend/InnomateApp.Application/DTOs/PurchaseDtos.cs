using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.DTOs
{
    public class PurchaseResponseDto
    {
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseDetailResponseDto> PurchaseDetails { get; set; } = new();
    }

    public class PurchaseDetailResponseDto
    {
        public int PurchaseDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal RemainingQty { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class CreatePurchaseDto
    {
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public string InvoiceNo { get; set; } = string.Empty;
        public string Notes { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreatePurchaseDetailDto> PurchaseDetails { get; set; } = new();
    }

    public class CreatePurchaseDetailDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [StringLength(100)]
        public string? BatchNo { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitCost { get; set; }
    }
}
