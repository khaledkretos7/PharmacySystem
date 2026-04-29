namespace PharmacySystem.Domain.Common;

/// <summary>
/// Base entity for all domain objects.
/// Provides Id, soft-delete flag, and full audit trail.
/// </summary>
public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;

    /// <summary>UTC timestamp of record creation.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of last modification.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Soft-delete flag — never physically delete rows.</summary>
    public bool IsDeleted { get; set; } = false;
}
