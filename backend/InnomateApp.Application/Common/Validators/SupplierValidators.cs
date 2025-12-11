using FluentValidation;
using InnomateApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Common.Validators
{
    public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
    {
        public CreateSupplierDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required")
                .MaximumLength(200).WithMessage("Supplier name cannot exceed 200 characters");

            RuleFor(x => x.ContactPerson)
                .MaximumLength(100).WithMessage("Contact person name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.ContactPerson));

            RuleFor(x => x.Phone)
                .MaximumLength(100).WithMessage("Phone number cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));
        }
    }

    public class UpdateSupplierDtoValidator : AbstractValidator<UpdateSupplierDto>
    {
        public UpdateSupplierDtoValidator()
        {
            Include(new CreateSupplierDtoValidator());

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier ID must be greater than 0");
        }
    }

}
