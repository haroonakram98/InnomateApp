using InnomateApp.Application.DTOs.Returns.Requests;
using InnomateApp.Application.DTOs.Returns.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces.Services
{
    public interface IReturnService
    {
        Task<ReturnResponse> CreateReturnAsync(CreateReturnRequest request);
        Task<IEnumerable<ReturnResponse>> GetReturnsBySaleIdAsync(int saleId);
        Task<ReturnResponse?> GetReturnByIdAsync(int returnId);
    }
}
