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
            var products = await _productRepo.GetProductsWithCategoryAsync();

            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                SKU = p.SKU,
                CategoryId = p.CategoryId, // Fixed: Added missing CategoryId
                CategoryName = p.Category?.Name ?? string.Empty,
                DefaultSalePrice = p.DefaultSalePrice,
                ReorderLevel = p.ReorderLevel,
                IsActive = p.IsActive,
                //StockBalance = p.StockSummary?.Balance ?? 0,
                //AverageCost = p.StockSummary?.AverageCost ?? 0,
                //TotalValue = p.StockSummary?.TotalValue ?? 0
            }).ToList();
        }

        // ✅ Get single product by ID
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                return null;

            return _mapper.Map<ProductDto>(product);
        }

        // ✅ Create new product + initialize StockSummary
        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var createdProduct = await _productRepo.AddAsync(product);
            

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
            //await _stockRepo.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        // ✅ Update product details
        public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            _mapper.Map(dto, product);
            
            // Fixed: Await the update task properly
            await _productRepo.UpdateAsync(product);
            
            // Re-fetch to get included data (Category, etc)
            var updatedProduct = await _productRepo.GetProductWithStockInfoAsync(product.ProductId);

            // Return the updated DTO so the store can update its state
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        // ✅ Delete product (and its stock summary)
        public async Task<int> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdtWithStockSummaryAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (product.StockSummary != null)
                await _stockRepo.DeleteAsync(product.StockSummary);

            await _productRepo.DeleteAsync(product);
            return id;
        }
        public Task DeactivateAsync(int id)
        {
            return _productRepo.DeactivateAsync(id);
        }

        public async Task<IEnumerable<ProductStockDto>> GetProductsForSale()
        {
            var products = await _productRepo.GetProductsWithCategoryAsync();

            return products.Select(p => new ProductStockDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SKU = p.SKU,
                ReorderLevel = p.ReorderLevel,
                IsActive = p.IsActive,
                DefaultSalePrice = p.DefaultSalePrice,
                StockSummary = p.StockSummary == null
            ? null
            : new StockSummaryDto
            {
                ProductId = p.StockSummary.ProductId,
                Balance = p.StockSummary.Balance,
                TotalIn = p.StockSummary.TotalIn,
                TotalOut = p.StockSummary.TotalOut,
                AverageCost = p.StockSummary.AverageCost,
                TotalValue = p.StockSummary.TotalValue,
                LastUpdated = p.StockSummary.LastUpdated
            }
            }).ToList();
        }
    }
}
