using PharmacySystem.Domain.Common;

namespace PharmacySystem.Domain.Entities;

/// <summary>
/// Customer who purchases medicines (walk-in or registered).
/// </summary>
public class Customer : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? InsuranceNumber { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? Allergies { get; set; }

    /// <summary>Cumulative loyalty / reward points.</summary>
    public int LoyaltyPoints { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // ── Navigation ──────────────────────────────
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
