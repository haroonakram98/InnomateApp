using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllWithStockAsync();
            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                SKU = p.SKU,
                CategoryName = p.Category.Name,
                DefaultSalePrice = p.DefaultSalePrice,
                ReorderLevel = p.ReorderLevel,
                IsActive = p.IsActive,
                StockBalance = p.StockSummary?.Balance ?? 0
            }).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdWithCategoryAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _productRepo.AddAsync(product);
            return await _productRepo.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null) throw new Exception("Product not found");

            _mapper.Map(dto, product);
            _productRepo.Update(product);
            return await _productRepo.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            _productRepo.Delete(product);
            return await _productRepo.SaveChangesAsync();
        }
    }
}
