using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.DTOs.Requests
{
    public class CreateSupplierRequest
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
        public string ContactPerson { get; set; }

        public string Notes { get; set; }
    }
}
