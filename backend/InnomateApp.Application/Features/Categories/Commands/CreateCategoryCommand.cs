using AutoMapper;
using FluentValidation;
using InnomateApp.Application.Common;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public CreateCategoryDto CategoryDto { get; set; } = null!;
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryDto)
                .NotNull()
                .SetValidator(new CreateCategoryDtoValidator());
        }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(
            IUnitOfWork uow, 
            IMapper mapper, 
            ITenantProvider tenantProvider,
            ILogger<CreateCategoryCommandHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating category: {CategoryName}", request.CategoryDto.Name);

                var tenantId = _tenantProvider.GetTenantId();
                var category = Category.Create(
                    tenantId,
                    request.CategoryDto.Name,
                    request.CategoryDto.Description
                );

                await _uow.Categories.AddAsync(category);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category created successfully: {CategoryId}", category.CategoryId);

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Result<CategoryDto>.Success(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {CategoryName}", request.CategoryDto.Name);
                return Result<CategoryDto>.Failure("An unexpected error occurred while creating the category.");
            }
        }
    }
}
