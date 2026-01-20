using System;

namespace InnomateApp.Application.DTOs.Suppliers.Responses
{
    public class SupplierDetailResponse : SupplierResponse
    {
        public int TotalPurchases { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int PendingPurchases { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
    }
}
