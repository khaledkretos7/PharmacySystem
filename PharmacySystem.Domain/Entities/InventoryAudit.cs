using PharmacySystem.Domain.Common;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Immutable ledger of every stock movement.
/// One row is written for every StockIn / StockOut / Adjustment / etc.
/// Never updated — append-only audit trail.
/// </summary>
public class InventoryAudit : BaseEntity<long>
{
    public AuditActionType ActionType { get; set; }

    /// <summary>Positive = stock added; Negative = stock removed.</summary>
    public int QuantityChanged { get; set; }

    /// <summary>Stock level immediately BEFORE this action.</summary>
    public int QuantityBefore { get; set; }

    /// <summary>Stock level immediately AFTER this action.</summary>
    public int QuantityAfter { get; set; }

    public string? BatchNumber  { get; set; }
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Reference to the source document: Invoice number, PO number, or
    /// adjustment note — keeps the audit human-readable.
    /// </summary>
    public string? ReferenceDocument { get; set; }

    public string? Notes { get; set; }

    // ── Foreign Keys ────────────────────────────
    public int    MedicineId    { get; set; }
    public string PerformedByUserId { get; set; } = string.Empty;

    // ── Navigation ──────────────────────────────
    public Medicine Medicine { get; set; } = null!;
}
