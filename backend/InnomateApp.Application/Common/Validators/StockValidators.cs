using FluentValidation;
using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Common.Validators
{
    public class StockMovementValidator : AbstractValidator<StockMovementDto>
    {
        public StockMovementValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.TransactionType).NotEmpty().Must(x => "PSA".Contains(x));
            RuleFor(x => x.Quantity).NotEqual(0);
            RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
        }
    }

    public class FIFOSaleRequestValidator : AbstractValidator<FIFOSaleRequestDto>
    {
        public FIFOSaleRequestValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.SaleReferenceId).GreaterThan(0);
        }
    }
}
