using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<int> UpdateAsync(UpdateProductDto dto);
        Task<int> DeleteAsync(int id);
        Task DeactivateAsync(int id);
        Task<IEnumerable<ProductStockDto>> GetProductsForSale();
    }
}
