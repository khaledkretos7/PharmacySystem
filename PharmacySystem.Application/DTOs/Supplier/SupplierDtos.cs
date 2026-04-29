namespace PharmacySystem.Application.DTOs.Supplier;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreateSupplierDto
{
    public string  Name          { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string  Phone         { get; set; } = string.Empty;
    public string? Email         { get; set; }
    public string? Address       { get; set; }
    public string? TaxNumber     { get; set; }
    public string? LicenseNumber { get; set; }
}

public class UpdateSupplierDto : CreateSupplierDto
{
    public int  Id       { get; set; }
    public bool IsActive { get; set; } = true;
}

// ── Responses ───────────────────────────────────────────────────────────────

public class SupplierResponseDto
{
    public int      Id             { get; set; }
    public string   Name           { get; set; } = string.Empty;
    public string?  ContactPerson  { get; set; }
    public string   Phone          { get; set; } = string.Empty;
    public string?  Email          { get; set; }
    public string?  Address        { get; set; }
    public string?  TaxNumber      { get; set; }
    public string?  LicenseNumber  { get; set; }
    public bool     IsActive       { get; set; }
    public int      MedicineCount  { get; set; }
    public int      PurchaseCount  { get; set; }
    public DateTime CreatedAt      { get; set; }
}
