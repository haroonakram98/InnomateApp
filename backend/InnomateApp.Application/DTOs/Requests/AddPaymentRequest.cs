using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Application.DTOs.Sales.Requests
{
    public class AddPaymentRequest
    {
        [Required]
        public int SaleId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "Cash";

        public string? ReferenceNo { get; set; }
    }
}
