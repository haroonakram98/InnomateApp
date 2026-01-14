using FluentValidation;
using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Common.Validators
{
    public class CreatePurchaseDetailDtoValidator : AbstractValidator<CreatePurchaseDetailDto>
    {
        public CreatePurchaseDetailDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.UnitCost)
                .GreaterThan(0).WithMessage("Unit cost must be greater than 0");

            RuleFor(x => x.BatchNo)
                .MaximumLength(100).WithMessage("Batch number cannot exceed 100 characters");

            // Expiry date validation (optional but good practice)
            RuleFor(x => x.ExpiryDate)
                .Must(date => date == null || date > DateTime.UtcNow)
                .WithMessage("Expiry date must be in the future")
                .When(x => x.ExpiryDate.HasValue);
        }
    }

    public class CreatePurchaseDtoValidator : AbstractValidator<CreatePurchaseDto>
    {
        public CreatePurchaseDtoValidator()
        {
            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier ID is required");

            RuleFor(x => x.PurchaseDate)
                .NotEmpty().WithMessage("Purchase date is required")
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5)).WithMessage("Purchase date cannot be in the future");

            RuleFor(x => x.CreatedBy)
                .GreaterThan(0).WithMessage("Creator ID is required");

            RuleFor(x => x.PurchaseDetails)
                .NotEmpty().WithMessage("At least one purchase item is required")
                .Must(details => details != null && details.Count > 0).WithMessage("At least one purchase item is required");

            RuleForEach(x => x.PurchaseDetails)
                .SetValidator(new CreatePurchaseDetailDtoValidator());
        }
    }
}
