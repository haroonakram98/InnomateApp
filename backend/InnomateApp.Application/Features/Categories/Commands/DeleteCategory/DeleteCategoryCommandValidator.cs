using FluentValidation;

namespace InnomateApp.Application.Features.Categories.Commands.DeleteCategory
{
    /// <summary>
    /// Validator for DeleteCategoryCommand
    /// </summary>
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Category ID must be greater than 0");
        }
    }
}
