using FluentValidation;

namespace InnomateApp.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Request.ProductId).GreaterThan(0);
            
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200);

            RuleFor(x => x.Request.CategoryId).GreaterThan(0);
             
             RuleFor(x => x.Request.DefaultSalePrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Request.ReorderLevel)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Request.SKU)
                 .MaximumLength(50);
        }
    }
}
