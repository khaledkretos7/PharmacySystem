using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.DTOs.Inventory;

// ── Requests ────────────────────────────────────────────────────────────────

/// <summary>Manual stock adjustment (positive = add, negative = remove).</summary>
public class StockAdjustmentDto
{
    public int     MedicineId        { get; set; }
    public int     QuantityAdjusted  { get; set; }  // can be negative
    public string? BatchNumber       { get; set; }
    public DateTime? ExpiryDate      { get; set; }
    public string? Notes             { get; set; }
    public string? ReferenceDocument { get; set; }
}

/// <summary>Physical count line for comparing system vs actual stock.</summary>
public class PhysicalCountItemDto
{
    public int MedicineId     { get; set; }
    public int ActualQuantity { get; set; }
}

/// <summary>Full physical count submission.</summary>
public class PhysicalCountDto
{
    public string? Notes { get; set; }
    public List<PhysicalCountItemDto> Items { get; set; } = [];
}

// ── Responses ───────────────────────────────────────────────────────────────

public class InventoryAuditResponseDto
{
    public long     Id                { get; set; }
    public int      MedicineId        { get; set; }
    public string   MedicineName      { get; set; } = string.Empty;
    public string   ActionType        { get; set; } = string.Empty;
    public int      QuantityChanged   { get; set; }
    public int      QuantityBefore    { get; set; }
    public int      QuantityAfter     { get; set; }
    public string?  BatchNumber       { get; set; }
    public DateTime? ExpiryDate       { get; set; }
    public string?  ReferenceDocument { get; set; }
    public string?  Notes             { get; set; }
    public string   PerformedByUser   { get; set; } = string.Empty;
    public DateTime CreatedAt         { get; set; }
}

/// <summary>Result of comparing system stock with physical count.</summary>
public class StockVarianceDto
{
    public int     MedicineId      { get; set; }
    public string  MedicineName    { get; set; } = string.Empty;
    public int     SystemQuantity  { get; set; }
    public int     ActualQuantity  { get; set; }
    public int     Variance        { get; set; }   // Actual - System
    public decimal VarianceValue   { get; set; }   // Variance * CostPrice
    public bool    HasDiscrepancy  => Variance != 0;
}

public class PhysicalCountResultDto
{
    public DateTime AuditDate               { get; set; }
    public int      TotalItemsChecked       { get; set; }
    public int      DiscrepanciesFound      { get; set; }
    public decimal  TotalVarianceValue      { get; set; }
    public List<StockVarianceDto> Variances { get; set; } = [];
}
