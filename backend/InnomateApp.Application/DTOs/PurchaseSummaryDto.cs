using System;

namespace InnomateApp.Application.DTOs
{
    public class PurchaseSummaryDto
    {
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
