using System;
using System.Collections.Generic;

namespace InnomateApp.Application.DTOs
{
    public class SupplierDto
    {
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSupplierDto : CreateSupplierDto
    {
        public int SupplierId { get; set; }
    }

    public class SupplierWithStatsDto : SupplierDto
    {
        public int TotalPurchases { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int PendingPurchases { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
    }

    public class SupplierDetailDto : SupplierDto
    {
        public int TotalPurchases { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int PendingPurchases { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public List<PurchaseSummaryDto> RecentPurchases { get; set; } = new List<PurchaseSummaryDto>();
    }

    public class PurchaseSummaryDto
    {
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}