using FluentValidation;
using PharmacySystem.Application.DTOs.Medicine;

namespace PharmacySystem.Application.Validators;

public class CreateMedicineDtoValidator : AbstractValidator<CreateMedicineDto>
{
    public CreateMedicineDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Medicine name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.GenericName)
            .NotEmpty().WithMessage("Generic name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .MaximumLength(50);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Cost price must be >= 0.");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0).WithMessage("Selling price must be > 0.")
            .GreaterThanOrEqualTo(x => x.CostPrice).WithMessage("Selling price must be >= cost price.");

        RuleFor(x => x.QuantityInStock)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative.");

        RuleFor(x => x.MaxStockLevel)
            .GreaterThan(x => x.ReorderLevel).WithMessage("Max stock must be > reorder level.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category is required.");

        RuleFor(x => x.ExpiryDate)
            .Must(d => !d.HasValue || d.Value > DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future.");

        RuleFor(x => x.Barcode)
            .MaximumLength(100).When(x => x.Barcode != null);
    }
}

public class UpdateMedicineDtoValidator : AbstractValidator<UpdateMedicineDto>
{
    public UpdateMedicineDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid medicine Id.");
        Include(new CreateMedicineDtoValidator());
    }
}
