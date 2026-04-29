using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Line item within a purchase order.
/// </summary>
public class PurchaseItem : BaseEntity<int>
{
    public int      QuantityOrdered  { get; set; }
    public int      QuantityReceived { get; set; }
    public decimal  UnitCost         { get; set; }
    public decimal  DiscountRate     { get; set; }
    public decimal  DiscountAmount   { get; set; }
    public decimal  TaxRate          { get; set; }
    public decimal  TaxAmount        { get; set; }
    public decimal  TotalCost        { get; set; }

    /// <summary>Batch / lot number from the supplier's delivery.</summary>
    public string?  BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // ── Foreign Keys ────────────────────────────
    public int PurchaseId { get; set; }
    public int MedicineId { get; set; }

    // ── Navigation ──────────────────────────────
    public Purchase Purchase { get; set; } = null!;
    public Medicine Medicine { get; set; } = null!;
}
