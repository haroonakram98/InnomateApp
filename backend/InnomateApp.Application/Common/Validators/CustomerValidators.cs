using FluentValidation;
using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Common.Validators
{
    public class CustomerValidatorBase<T> : AbstractValidator<T> where T : CreateCustomerDto
    {
        public CustomerValidatorBase()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
        }
    }

    public class CreateCustomerDtoValidator : CustomerValidatorBase<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator() : base() { }
    }

    public class UpdateCustomerDtoValidator : CustomerValidatorBase<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator() : base()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer ID is required for updates");
        }
    }
}
