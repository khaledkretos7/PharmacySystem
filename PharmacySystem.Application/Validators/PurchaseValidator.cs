using FluentValidation;
using PharmacySystem.Application.DTOs.Purchase;

namespace PharmacySystem.Application.Validators;

public class CreatePurchaseDtoValidator : AbstractValidator<CreatePurchaseDto>
{
    public CreatePurchaseDtoValidator()
    {
        RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage("Supplier is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Purchase must have at least one item.");

        RuleForEach(x => x.Items).SetValidator(new CreatePurchaseItemDtoValidator());
    }
}

public class CreatePurchaseItemDtoValidator : AbstractValidator<CreatePurchaseItemDto>
{
    public CreatePurchaseItemDtoValidator()
    {
        RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage("Invalid medicine.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be > 0.");
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).WithMessage("Unit cost cannot be negative.");
        RuleFor(x => x.DiscountRate).InclusiveBetween(0, 1).WithMessage("Discount rate must be 0–1.");
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 1).WithMessage("Tax rate must be 0–1.");

        RuleFor(x => x.ExpiryDate)
            .Must(d => !d.HasValue || d.Value > DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiryDate.HasValue);
    }
}

public class ReceivePurchaseDtoValidator : AbstractValidator<ReceivePurchaseDto>
{
    public ReceivePurchaseDtoValidator()
    {
        RuleFor(x => x.PurchaseId).GreaterThan(0).WithMessage("Invalid purchase.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("Must specify at least one received item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.PurchaseItemId).GreaterThan(0);
            item.RuleFor(i => i.QuantityReceived).GreaterThan(0);
        });
    }
}
