using FluentValidation;
using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Common.Validators
{
    /// <summary>
    /// Shared base validation rules for Suppliers to ensure consistency 
    /// between Create and Update operations.
    /// </summary>
    public class SupplierValidatorBase<T> : AbstractValidator<T> where T : CreateSupplierDto
    {
        public SupplierValidatorBase()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required")
                .MaximumLength(200).WithMessage("Supplier name cannot exceed 200 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

            RuleFor(x => x.ContactPerson)
                .MaximumLength(100).WithMessage("Contact person name cannot exceed 100 characters");
        }
    }

    public class CreateSupplierDtoValidator : SupplierValidatorBase<CreateSupplierDto>
    {
        public CreateSupplierDtoValidator() : base() { }
    }

    public class UpdateSupplierDtoValidator : SupplierValidatorBase<UpdateSupplierDto>
    {
        public UpdateSupplierDtoValidator() : base()
        {
            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier ID is required for updates");
        }
    }
}
