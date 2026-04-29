using FluentValidation;
using PharmacySystem.Application.DTOs.Invoice;

namespace PharmacySystem.Application.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator()
    {
        RuleFor(x => x.AmountPaid)
            .GreaterThanOrEqualTo(0).WithMessage("Amount paid cannot be negative.");

        RuleFor(x => x.GlobalDiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Invoice must have at least one item.");

        RuleForEach(x => x.Items).SetValidator(new CreateInvoiceItemDtoValidator());
    }
}

public class CreateInvoiceItemDtoValidator : AbstractValidator<CreateInvoiceItemDto>
{
    public CreateInvoiceItemDtoValidator()
    {
        RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage("Invalid medicine.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be > 0.");

        RuleFor(x => x.DiscountRate)
            .InclusiveBetween(0, 1).WithMessage("Discount rate must be between 0 and 1.");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 1).WithMessage("Tax rate must be between 0 and 1.");
    }
}

public class CreateInvoiceReturnDtoValidator : AbstractValidator<CreateInvoiceReturnDto>
{
    public CreateInvoiceReturnDtoValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invalid invoice.");
        RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage("Invalid medicine.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Return quantity must be > 0.");
    }
}
