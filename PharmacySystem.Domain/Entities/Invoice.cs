using PharmacySystem.Domain.Common;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Sales invoice (point-of-sale transaction header).
/// </summary>
public class Invoice : BaseEntity<int>
{
    /// <summary>Human-readable invoice number — e.g. INV-20240001.</summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public decimal SubTotal      { get; set; }
    public decimal DiscountAmount{ get; set; }
    public decimal TaxAmount     { get; set; }
    public decimal TotalAmount   { get; set; }
    public decimal AmountPaid    { get; set; }
    public decimal ChangeAmount  { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public InvoiceStatus Status        { get; set; } = InvoiceStatus.Confirmed;

    /// <summary>Optional doctor name for prescription invoices.</summary>
    public string? PrescriptionReference { get; set; }
    public string? Notes { get; set; }

    // ── Foreign Keys ────────────────────────────
    /// <summary>Nullable — supports anonymous / walk-in customers.</summary>
    public int? CustomerId { get; set; }

    /// <summary>ASP.NET Identity user (cashier) who processed this sale.</summary>
    public string UserId { get; set; } = string.Empty;

    // ── Navigation ──────────────────────────────
    public Customer?                  Customer       { get; set; }
    public ICollection<InvoiceItem>   Items          { get; set; } = new List<InvoiceItem>();
    public ICollection<InvoiceReturn> Returns        { get; set; } = new List<InvoiceReturn>();
}
