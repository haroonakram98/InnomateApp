using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Application.DTOs.Products.Requests
{
    public class UpdateProductRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public bool IsActive { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal DefaultSalePrice { get; set; }
    }
}
