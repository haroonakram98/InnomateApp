using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Application.DTOs.Returns.Requests
{
    public class CreateReturnRequest
    {
        [Required]
        public int SaleId { get; set; }
        
        public string? Reason { get; set; }
        
        [Required]
        [MinLength(1)]
        public List<ReturnDetailRequest> ReturnDetails { get; set; } = new();
    }

    public class ReturnDetailRequest
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}
