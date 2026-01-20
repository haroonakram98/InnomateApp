using FluentValidation;

namespace InnomateApp.Application.Features.Categories.Commands.CreateCategory
{
    /// <summary>
    /// Validator for CreateCategoryCommand
    /// Runs before the handler via ValidationBehavior pipeline
    /// </summary>
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("Request cannot be null");

            RuleFor(x => x.Request.Name)
                .NotEmpty()
                .WithMessage("Category name is required")
                .MaximumLength(100)
                .WithMessage("Category name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-\&]+$")
                .WithMessage("Category name can only contain letters, numbers, spaces, hyphens, and ampersands");

            RuleFor(x => x.Request.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Request.Description));
        }
    }
}
