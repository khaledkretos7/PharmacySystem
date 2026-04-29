using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.DTOs.Medicine;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreateMedicineDto
{
    public string     Name                { get; set; } = string.Empty;
    public string     GenericName         { get; set; } = string.Empty;
    public string?    Barcode             { get; set; }
    public string?    Description         { get; set; }
    public string?    Manufacturer        { get; set; }
    public DosageForm DosageForm          { get; set; } = DosageForm.Tablet;
    public string?    Strength            { get; set; }
    public string     Unit                { get; set; } = string.Empty;
    public decimal    CostPrice           { get; set; }
    public decimal    SellingPrice        { get; set; }
    public int        QuantityInStock     { get; set; }
    public int        ReorderLevel        { get; set; } = 10;
    public int        MaxStockLevel       { get; set; } = 1000;
    public DateTime?  ExpiryDate          { get; set; }
    public string?    StorageConditions   { get; set; }
    public bool       RequiresPrescription{ get; set; } = false;
    public int        CategoryId          { get; set; }
    public int?       SupplierId          { get; set; }
}

public class UpdateMedicineDto : CreateMedicineDto
{
    public int            Id     { get; set; }
    public MedicineStatus Status { get; set; } = MedicineStatus.Active;
}

public class MedicineFilterDto
{
    public string?        SearchTerm   { get; set; }
    public int?           CategoryId   { get; set; }
    public int?           SupplierId   { get; set; }
    public MedicineStatus? Status      { get; set; }
    public bool?          LowStock     { get; set; }
    public bool?          NearExpiry   { get; set; }
    public int            PageNumber   { get; set; } = 1;
    public int            PageSize     { get; set; } = 20;
}

// ── Responses ───────────────────────────────────────────────────────────────

public class MedicineResponseDto
{
    public int            Id                  { get; set; }
    public string         Name                { get; set; } = string.Empty;
    public string         GenericName         { get; set; } = string.Empty;
    public string?        Barcode             { get; set; }
    public string?        Description         { get; set; }
    public string?        Manufacturer        { get; set; }
    public string         DosageForm          { get; set; } = string.Empty;
    public string?        Strength            { get; set; }
    public string         Unit                { get; set; } = string.Empty;
    public decimal        CostPrice           { get; set; }
    public decimal        SellingPrice        { get; set; }
    public int            QuantityInStock     { get; set; }
    public int            ReorderLevel        { get; set; }
    public int            MaxStockLevel       { get; set; }
    public DateTime?      ExpiryDate          { get; set; }
    public string?        StorageConditions   { get; set; }
    public bool           RequiresPrescription{ get; set; }
    public string         Status              { get; set; } = string.Empty;
    public int            CategoryId          { get; set; }
    public string         CategoryName        { get; set; } = string.Empty;
    public int?           SupplierId          { get; set; }
    public string?        SupplierName        { get; set; }
    public bool           IsLowStock          { get; set; }
    public bool           IsNearExpiry        { get; set; }
    public bool           IsExpired           { get; set; }
    public DateTime       CreatedAt           { get; set; }
    public DateTime?      UpdatedAt           { get; set; }
}

/// <summary>Lightweight version for list/dropdown responses.</summary>
public class MedicineListDto
{
    public int     Id           { get; set; }
    public string  Name         { get; set; } = string.Empty;
    public string  GenericName  { get; set; } = string.Empty;
    public string? Barcode      { get; set; }
    public string  Unit         { get; set; } = string.Empty;
    public decimal SellingPrice { get; set; }
    public int     QuantityInStock { get; set; }
    public bool    IsLowStock   { get; set; }
    public bool    IsExpired    { get; set; }
    public string  CategoryName { get; set; } = string.Empty;
    public string  Status       { get; set; } = string.Empty;
}
