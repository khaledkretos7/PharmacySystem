using PharmacySystem.Domain.Common;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Purchase order header — medicines bought from a supplier.
/// </summary>
public class Purchase : BaseEntity<int>
{
    /// <summary>Auto-generated purchase reference — e.g. PO-20240001.</summary>
    public string PurchaseNumber { get; set; } = string.Empty;

    /// <summary>Supplier's own invoice / reference number.</summary>
    public string? SupplierInvoiceNumber { get; set; }

    public DateTime PurchaseDate    { get; set; } = DateTime.UtcNow;
    public DateTime? ReceivedDate   { get; set; }
    public DateTime? ExpectedDate   { get; set; }

    public decimal SubTotal         { get; set; }
    public decimal DiscountAmount   { get; set; }
    public decimal TaxAmount        { get; set; }
    public decimal TotalAmount      { get; set; }
    public decimal AmountPaid       { get; set; }
    public decimal RemainingBalance { get; set; }

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Ordered;
    public string? Notes { get; set; }

    // ── Foreign Keys ────────────────────────────
    public int SupplierId { get; set; }

    /// <summary>Staff member who created this purchase order.</summary>
    public string CreatedByUserId { get; set; } = string.Empty;

    // ── Navigation ──────────────────────────────
    public Supplier                  Supplier { get; set; } = null!;
    public ICollection<PurchaseItem> Items    { get; set; } = new List<PurchaseItem>();
}
