using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int SaleId { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, Wallet
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? ReferenceNo { get; set; } // Optional for card/mobile

        public Sale? Sale { get; set; }
    }
}
