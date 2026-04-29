using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Expense category — e.g. Rent, Utilities, Salaries.
/// </summary>
public class ExpenseCategory : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // ── Navigation ──────────────────────────────
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
