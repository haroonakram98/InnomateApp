using AutoMapper;
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
    {
        public int Id { get; set; }

        public GetCategoryByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _uow.Categories.GetByIdAsync(request.Id);
            if (category == null)
                return Result<CategoryDto>.Failure("Category not found.");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(categoryDto);
        }
    }
}
