using FluentValidation;
using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Common.Validators
{
    public class CategoryValidatorBase<T> : AbstractValidator<T> where T : CreateCategoryDto
    {
        public CategoryValidatorBase()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }

    public class CreateCategoryDtoValidator : CategoryValidatorBase<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator() : base() { }
    }

    public class UpdateCategoryDtoValidator : CategoryValidatorBase<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator() : base()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID is required for updates");
        }
    }
}
