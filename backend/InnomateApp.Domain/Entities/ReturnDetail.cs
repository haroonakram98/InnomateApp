using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class ReturnDetail
    {
        public int ReturnDetailId { get; set; }
        public int ReturnId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public Return? Return { get; set; }
        public Product? Product { get; set; }
    }
}
