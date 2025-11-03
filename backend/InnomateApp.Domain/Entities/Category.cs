using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
