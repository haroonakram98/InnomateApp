using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.StockBalance, opt => opt.MapFrom(src => src.StockSummary != null ? src.StockSummary.Balance : 0));

            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
