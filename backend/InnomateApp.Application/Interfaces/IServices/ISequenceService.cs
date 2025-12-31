using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces.Services
{
    public interface ISequenceService
    {
        Task<string> GeneratePurchaseNumberAsync();
        Task<string> GenerateInvoiceNumberAsync();
        Task<string> GenerateBatchNumberAsync();
    }
}
