using FluentValidation;

namespace InnomateApp.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.Request.CategoryId)
                .GreaterThan(0).WithMessage("Valid Category ID is required");

            RuleFor(x => x.Request.DefaultSalePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative");

            RuleFor(x => x.Request.ReorderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative");

            RuleFor(x => x.Request.SKU)
                 .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters");
        }
    }
}
