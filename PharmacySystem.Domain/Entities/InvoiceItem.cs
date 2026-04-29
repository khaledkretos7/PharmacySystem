using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Line item within a sales invoice.
/// </summary>
public class InvoiceItem : BaseEntity<int>
{
    public int      Quantity      { get; set; }
    public decimal  UnitPrice     { get; set; }
    public decimal  DiscountRate  { get; set; }   // e.g. 0.10 = 10%
    public decimal  DiscountAmount{ get; set; }
    public decimal  TaxRate       { get; set; }   // e.g. 0.15 = 15%
    public decimal  TaxAmount     { get; set; }
    public decimal  TotalAmount   { get; set; }   // (UnitPrice * Qty) - Discount + Tax

    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // ── Foreign Keys ────────────────────────────
    public int InvoiceId  { get; set; }
    public int MedicineId { get; set; }

    // ── Navigation ──────────────────────────────
    public Invoice  Invoice  { get; set; } = null!;
    public Medicine Medicine { get; set; } = null!;
}
