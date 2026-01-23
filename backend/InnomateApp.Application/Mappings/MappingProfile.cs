using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.DTOs.Sales.Responses;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>();

            // Customer mappings
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            // Supplier mappings
            CreateMap<Supplier, SupplierDto>().ReverseMap();
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>();
            CreateMap<Supplier, SupplierWithStatsDto>();
            CreateMap<Supplier, SupplierDetailDto>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
            CreateMap<Product, ProductStockDto>()
                .IncludeBase<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();

            // Purchase mappings
            CreateMap<Purchase, PurchaseSummaryDto>();
            CreateMap<Purchase, PurchaseResponseDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null));
            CreateMap<PurchaseDetail, PurchaseDetailResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
            CreateMap<CreatePurchaseDto, Purchase>();
            CreateMap<CreatePurchaseDetailDto, PurchaseDetail>();

            // Stock mappings
            CreateMap<StockSummary, StockSummaryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
            
            CreateMap<StockTransaction, StockTransactionDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.ToString()));

            CreateMap<PurchaseDetail, FifoBatchDto>()
                .ForMember(dest => dest.PurchaseDetailId, opt => opt.MapFrom(src => src.PurchaseDetailId))
                .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.RemainingQty))
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.Purchase != null ? src.Purchase.PurchaseDate : DateTime.MinValue));

            // Sale mappings
            CreateMap<Sale, SaleDto>();
            CreateMap<SaleDetail, SaleDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.CostPrice, opt => opt.MapFrom(src => src.PurchaseDetail != null ? src.PurchaseDetail.UnitCost : 0))
                .ForMember(dest => dest.ProfitMargin, opt => opt.MapFrom(src =>
                    src.PurchaseDetail != null && src.UnitPrice > 0 ?
                    ((src.UnitPrice - src.PurchaseDetail.UnitCost) / src.UnitPrice) * 100 : 0));
            CreateMap<CreateSaleDto, Sale>();
            CreateMap<CreateSaleDetailDto, SaleDetail>();

            CreateMap<Sale, SaleResponse>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.SaleDetails, opt => opt.MapFrom(src => src.SaleDetails))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));

            CreateMap<SaleDetail, SaleDetailResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ProfitMargin, opt => opt.MapFrom(src =>
                    src.Total > 0 ? (src.Profit / src.Total) * 100 : 0))
                .ForMember(dest => dest.UsedBatches, opt => opt.MapFrom(src => src.UsedBatches));
            
            CreateMap<SaleDetailBatch, FIFOLayerDto>();
            CreateMap<Customer, CustomerShortResponse>();
            CreateMap<Payment, PaymentResponse>();
        }
    }
}
