using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>
    {
        public string? Search { get; set; }
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Category> categories = await _uow.Categories.GetAllAsync();
            
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                categories = categories.Where(c => 
                    c.Name.ToLower().Contains(search) || 
                    (c.Description != null && c.Description.ToLower().Contains(search))
                );
            }

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Result<IEnumerable<CategoryDto>>.Success(categoryDtos);
        }
    }
}
