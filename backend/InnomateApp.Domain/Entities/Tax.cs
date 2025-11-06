using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Tax
    {
        public int TaxId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
