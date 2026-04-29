using PharmacySystem.Domain.Common;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Records a full or partial return against a sales invoice.
/// Restores stock and optionally issues a refund.
/// </summary>
public class InvoiceReturn : BaseEntity<int>
{
    /// <summary>Auto-generated return reference — e.g. RET-20240001.</summary>
    public string ReturnNumber { get; set; } = string.Empty;

    public DateTime ReturnDate    { get; set; } = DateTime.UtcNow;
    public int      Quantity      { get; set; }
    public decimal  UnitPrice     { get; set; }
    public decimal  RefundAmount  { get; set; }
    public ReturnReason Reason    { get; set; } = ReturnReason.Other;
    public string?  Notes         { get; set; }

    /// <summary>Whether the refund was actually issued to the customer.</summary>
    public bool IsRefunded { get; set; } = false;

    // ── Foreign Keys ────────────────────────────
    public int InvoiceId  { get; set; }
    public int MedicineId { get; set; }

    /// <summary>Staff member who processed the return.</summary>
    public string ProcessedByUserId { get; set; } = string.Empty;

    // ── Navigation ──────────────────────────────
    public Invoice  Invoice  { get; set; } = null!;
    public Medicine Medicine { get; set; } = null!;
}
