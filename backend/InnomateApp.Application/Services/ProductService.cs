using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IStockSummaryRepository _stockRepo;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepo,
            IStockSummaryRepository stockRepo,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _stockRepo = stockRepo;
            _mapper = mapper;
        }

        // ✅ Get all products with stock
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllWithStockAsync();

            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                SKU = p.SKU,
                CategoryName = p.Category?.Name ?? string.Empty,
                DefaultSalePrice = p.DefaultSalePrice,
                ReorderLevel = p.ReorderLevel,
                IsActive = p.IsActive,
                StockBalance = p.StockSummary?.Balance ?? 0,
                AverageCost = p.StockSummary?.AverageCost ?? 0,
                TotalValue = p.StockSummary?.TotalValue ?? 0
            }).ToList();
        }

        // ✅ Get single product by ID
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdWithCategoryAsync(id);
            if (product == null)
                return null;

            return _mapper.Map<ProductDto>(product);
        }

        // ✅ Create new product + initialize StockSummary
        public async Task<int> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            // Initialize stock summary
            var stock = new StockSummary
            {
                ProductId = product.ProductId,
                Balance = 0,
                TotalIn = 0,
                TotalOut = 0,
                AverageCost = 0,
                TotalValue = 0
            };
            await _stockRepo.AddAsync(stock);
            await _stockRepo.SaveChangesAsync();

            return product.ProductId;
        }

        // ✅ Update product details
        public async Task<int> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            _mapper.Map(dto, product);
            _productRepo.Update(product);
            return await _productRepo.SaveChangesAsync();
        }

        // ✅ Delete product (and its stock summary)
        public async Task<int> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdWithStockAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (product.StockSummary != null)
                await _stockRepo.DeleteAsync(product.StockSummary);

            _productRepo.Delete(product);
            return await _productRepo.SaveChangesAsync();
        }
    }
}
