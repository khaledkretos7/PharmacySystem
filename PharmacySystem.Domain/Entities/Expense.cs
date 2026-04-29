using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Operational expense record — rent, utilities, salaries, etc.
/// </summary>
public class Expense : BaseEntity<int>
{
    public string   Title        { get; set; } = string.Empty;
    public string?  Description  { get; set; }
    public decimal  Amount       { get; set; }
    public DateTime ExpenseDate  { get; set; } = DateTime.UtcNow;
    public string?  Reference    { get; set; }   // Receipt / voucher number
    public bool     IsRecurring  { get; set; } = false;
    public string?  Notes        { get; set; }

    // ── Foreign Keys ────────────────────────────
    public int    ExpenseCategoryId { get; set; }

    /// <summary>Staff member who recorded this expense.</summary>
    public string RecordedByUserId { get; set; } = string.Empty;

    // ── Navigation ──────────────────────────────
    public ExpenseCategory ExpenseCategory { get; set; } = null!;
}
