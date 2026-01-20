using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Application.DTOs.Suppliers.Requests
{
    public class CreateSupplierRequest
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(200), EmailAddress]
        public string Email { get; set; }

        [StringLength(20), Required]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        public string Notes { get; set; }
    }
}
