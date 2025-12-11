// Domain/Entities/Supplier.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InnomateApp.Domain.Entities
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        // Domain methods
        public void Update(string name, string email, string phone, string address = null, string contactPerson = null, string notes = null)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            ContactPerson = contactPerson;
            Notes = notes;
            UpdatedAt = DateTime.Now;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.Now;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.Now;
        }

        public void ToggleStatus()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.Now;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }
    }
}