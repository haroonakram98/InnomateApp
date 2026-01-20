using FluentValidation;

namespace InnomateApp.Application.Features.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Request.SupplierId)
                 .GreaterThan(0).WithMessage("Supplier ID must be greater than 0");

            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Supplier name is required")
                .MaximumLength(200).WithMessage("Supplier name cannot exceed 200 characters");

            RuleFor(x => x.Request.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Request.Email));

            RuleFor(x => x.Request.Phone)
                .MaximumLength(100).WithMessage("Phone number cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Request.Phone));

            RuleFor(x => x.Request.ContactPerson)
                .MaximumLength(100).WithMessage("Contact person name cannot exceed 100 characters");

            RuleFor(x => x.Request.Address)
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters");
        }
    }
}
