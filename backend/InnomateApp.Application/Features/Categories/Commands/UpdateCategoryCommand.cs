using AutoMapper;
using FluentValidation;
using InnomateApp.Application.Common;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public int Id { get; set; }
        public UpdateCategoryDto CategoryDto { get; set; } = null!;
    }

    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.CategoryDto)
                .NotNull()
                .SetValidator(new UpdateCategoryDtoValidator());
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<UpdateCategoryCommandHandler> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _uow.Categories.GetByIdAsync(request.Id);
                if (category == null)
                    return Result<CategoryDto>.Failure("Category not found.");

                _logger.LogInformation("Updating category: {CategoryName} (ID: {CategoryId})", category.Name, category.CategoryId);

                category.Update(
                    request.CategoryDto.Name,
                    request.CategoryDto.Description
                );

                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category updated successfully: {CategoryId}", category.CategoryId);

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Result<CategoryDto>.Success(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", request.Id);
                return Result<CategoryDto>.Failure("An unexpected error occurred while updating the category.");
            }
        }
    }
}
