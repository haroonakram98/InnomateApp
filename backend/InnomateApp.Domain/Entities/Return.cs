using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Return
    {
        public int ReturnId { get; set; }
        public int SaleId { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
        public decimal TotalRefund { get; set; }
        public string? Reason { get; set; }

        public Sale? Sale { get; set; }
        public ICollection<ReturnDetail>? ReturnDetails { get; set; }
    }
}
