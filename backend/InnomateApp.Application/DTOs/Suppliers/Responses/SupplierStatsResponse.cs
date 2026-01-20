using System;

namespace InnomateApp.Application.DTOs.Suppliers.Responses
{
    public class SupplierStatsResponse
    {
        public int TotalPurchases { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int PendingPurchases { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public decimal AveragePurchaseAmount { get; set; }
        public int CompletedPurchases { get; set; }
    }
}
