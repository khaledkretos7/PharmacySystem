using FluentValidation;
using PharmacySystem.Application.DTOs.Customer;
using PharmacySystem.Application.DTOs.Expense;
using PharmacySystem.Application.DTOs.Category;
using PharmacySystem.Application.DTOs.Supplier;
using PharmacySystem.Application.DTOs.Inventory;

namespace PharmacySystem.Application.Validators;

// ── Customer ─────────────────────────────────────────────────────────────────
public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone != null);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of birth must be in the past.");
    }
}

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        Include(new CreateCustomerDtoValidator());
    }
}

// ── Supplier ─────────────────────────────────────────────────────────────────
public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

public class UpdateSupplierDtoValidator : AbstractValidator<UpdateSupplierDto>
{
    public UpdateSupplierDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        Include(new CreateSupplierDtoValidator());
    }
}

// ── Category ─────────────────────────────────────────────────────────────────
public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

// ── Expense ──────────────────────────────────────────────────────────────────
public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Expense amount must be > 0.");
        RuleFor(x => x.ExpenseCategoryId).GreaterThan(0).WithMessage("Category is required.");
        RuleFor(x => x.ExpenseDate).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Expense date cannot be far in the future.");
    }
}

// ── Inventory ─────────────────────────────────────────────────────────────────
public class StockAdjustmentDtoValidator : AbstractValidator<StockAdjustmentDto>
{
    public StockAdjustmentDtoValidator()
    {
        RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage("Invalid medicine.");
        RuleFor(x => x.QuantityAdjusted).NotEqual(0).WithMessage("Adjustment quantity cannot be zero.");
    }
}
