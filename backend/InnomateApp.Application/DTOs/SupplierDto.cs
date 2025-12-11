using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;


        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
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