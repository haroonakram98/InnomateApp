using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Domain.Entities
{
    public class StockSummary
    {
        [Key]
        public int StockSummaryId { get; set; }
        public int ProductId { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Balance { get; set; }

        public decimal AverageCost { get; set; }

        public decimal TotalValue { get; set; }


        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public Product Product { get; set; } = null!;
    }
}
