using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? ContactPerson { get; set; }
        [MaxLength(100)]
        public string? Phone { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [MaxLength(300)]
        public string? Address { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
