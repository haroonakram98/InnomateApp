using System;

namespace InnomateApp.Application.DTOs.Suppliers.Responses
{
    public class SupplierResponse
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SupplierLookupResponse
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
    }
}
