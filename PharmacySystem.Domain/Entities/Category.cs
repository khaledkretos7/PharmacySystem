using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Medicine (drug) category — e.g. Antibiotics, Analgesics, Vitamins.
/// </summary>
public class Category : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // ── Navigation ──────────────────────────────
    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
