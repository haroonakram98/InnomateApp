using InnomateApp.Application.DTOs;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces.Services
{
    public interface IFifoService
    {
        Task<FIFOSaleResultDto> ProcessSaleWithFIFOAsync(
            int productId,
            decimal quantity,
            int saleDetailId,
            string reference,
            string notes);
    }
}
