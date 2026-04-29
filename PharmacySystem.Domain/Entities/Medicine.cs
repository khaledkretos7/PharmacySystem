using PharmacySystem.Domain.Common;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Core medicine (drug/product) entity.
/// Tracks stock, pricing, expiry, and classification.
/// </summary>
public class Medicine : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? Manufacturer { get; set; }

    public DosageForm DosageForm { get; set; } = DosageForm.Tablet;

    /// <summary>Strength / concentration — e.g. "500mg", "250ml".</summary>
    public string? Strength { get; set; }

    /// <summary>Unit of measure — Box / Strip / Bottle / Vial.</summary>
    public string Unit { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }

    /// <summary>Current quantity on hand.</summary>
    public int QuantityInStock { get; set; } = 0;

    /// <summary>Triggers low-stock alert below this level.</summary>
    public int ReorderLevel { get; set; } = 10;

    /// <summary>Maximum stock to hold (for over-stock detection).</summary>
    public int MaxStockLevel { get; set; } = 1000;

    public DateTime? ExpiryDate { get; set; }
    public string? StorageConditions { get; set; }

    /// <summary>Whether a doctor's prescription is required.</summary>
    public bool RequiresPrescription { get; set; } = false;

    public MedicineStatus Status { get; set; } = MedicineStatus.Active;

    // ── Foreign Keys ────────────────────────────
    public int CategoryId { get; set; }
    public int? SupplierId { get; set; }

    // ── Navigation ──────────────────────────────
    public Category Category { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public ICollection<InvoiceItem>    InvoiceItems    { get; set; } = new List<InvoiceItem>();
    public ICollection<PurchaseItem>   PurchaseItems   { get; set; } = new List<PurchaseItem>();
    public ICollection<InvoiceReturn>  InvoiceReturns  { get; set; } = new List<InvoiceReturn>();
    public ICollection<InventoryAudit> InventoryAudits { get; set; } = new List<InventoryAudit>();
}
