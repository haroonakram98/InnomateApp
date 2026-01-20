using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class SequenceService : ISequenceService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SequenceService> _logger;

        public SequenceService(IUnitOfWork uow, ILogger<SequenceService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<string> GeneratePurchaseNumberAsync()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var startOfDay = DateTime.Now.Date;
            var endOfDay = startOfDay.AddDays(1);

            var todaysPurchases = await _uow.Purchases.GetPurchasesByDateRangeAsync(startOfDay, endOfDay);
            var count = todaysPurchases.Count;

            var purchaseNumber = $"PUR-{today}-{count + 1:0000}";

            _logger.LogDebug("Generated purchase number: {PurchaseNumber}", purchaseNumber);

            return purchaseNumber;
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoiceNo = await _uow.Sales.GetLastInvoiceNoAsync();

            if (string.IsNullOrEmpty(lastInvoiceNo))
            {
                return "INV-1001";
            }

            // Expected format: INV-XXXX
            var parts = lastInvoiceNo.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1], out int number))
            {
                return $"INV-{number + 1}";
            }

            // Fallback if format is weird
            return $"INV-{DateTime.Now.Ticks}";
        }

        public async Task<string> GenerateBatchNumberAsync()
        {
            // Generate a concise unique batch number: BN-{yyyyMMdd}-{HHmmss}-{Random}
            // Or simpler: BN-{yyyyMMdd}-{Random4Chars} to keep it short but unique enough for UI
            var today = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random();
            var randSuffix = random.Next(1000, 9999);
            return $"BN-{today}-{randSuffix}";
        }
    }
}
