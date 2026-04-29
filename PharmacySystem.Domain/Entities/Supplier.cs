using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Supplier (vendor) who provides medicines to the pharmacy.
/// </summary>
public class Supplier : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? LicenseNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // ── Navigation ──────────────────────────────
    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
