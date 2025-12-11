using System;
using System.Collections.Generic;

namespace InnomateApp.Application.DTOs.Returns.Responses
{
    public class ReturnResponse
    {
        public int ReturnId { get; set; }
        public int SaleId { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalRefund { get; set; }
        public string? Reason { get; set; }
        public List<ReturnDetailResponse> ReturnDetails { get; set; } = new();
    }

    public class ReturnDetailResponse
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal RefundAmount { get; set; }
    }
}
