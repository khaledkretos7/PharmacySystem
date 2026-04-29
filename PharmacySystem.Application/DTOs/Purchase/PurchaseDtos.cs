using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.DTOs.Purchase;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreatePurchaseDto
{
    public int       SupplierId             { get; set; }
    public string?   SupplierInvoiceNumber  { get; set; }
    public DateTime  PurchaseDate           { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDate           { get; set; }
    public decimal   DiscountAmount         { get; set; } = 0;
    public string?   Notes                  { get; set; }
    public List<CreatePurchaseItemDto> Items{ get; set; } = [];
}

public class CreatePurchaseItemDto
{
    public int      MedicineId { get; set; }
    public int      Quantity   { get; set; }
    public decimal  UnitCost   { get; set; }
    public decimal  DiscountRate { get; set; } = 0;
    public decimal  TaxRate    { get; set; } = 0;
    public string?  BatchNumber{ get; set; }
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>Marks a purchase (or part of it) as received and updates stock.</summary>
public class ReceivePurchaseDto
{
    public int      PurchaseId { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    public List<ReceivePurchaseItemDto> Items { get; set; } = [];
}

public class ReceivePurchaseItemDto
{
    public int PurchaseItemId    { get; set; }
    public int QuantityReceived  { get; set; }
}

// ── Responses ───────────────────────────────────────────────────────────────

public class PurchaseResponseDto
{
    public int                           Id                    { get; set; }
    public string                        PurchaseNumber        { get; set; } = string.Empty;
    public string?                       SupplierInvoiceNumber { get; set; }
    public DateTime                      PurchaseDate          { get; set; }
    public DateTime?                     ReceivedDate          { get; set; }
    public DateTime?                     ExpectedDate          { get; set; }
    public int                           SupplierId            { get; set; }
    public string                        SupplierName          { get; set; } = string.Empty;
    public string                        Status                { get; set; } = string.Empty;
    public decimal                       SubTotal              { get; set; }
    public decimal                       DiscountAmount        { get; set; }
    public decimal                       TaxAmount             { get; set; }
    public decimal                       TotalAmount           { get; set; }
    public decimal                       AmountPaid            { get; set; }
    public decimal                       RemainingBalance      { get; set; }
    public string?                       Notes                 { get; set; }
    public List<PurchaseItemResponseDto> Items                 { get; set; } = [];
    public DateTime                      CreatedAt             { get; set; }
}

public class PurchaseItemResponseDto
{
    public int      Id               { get; set; }
    public int      MedicineId       { get; set; }
    public string   MedicineName     { get; set; } = string.Empty;
    public string   Unit             { get; set; } = string.Empty;
    public int      QuantityOrdered  { get; set; }
    public int      QuantityReceived { get; set; }
    public decimal  UnitCost         { get; set; }
    public decimal  DiscountAmount   { get; set; }
    public decimal  TaxAmount        { get; set; }
    public decimal  TotalCost        { get; set; }
    public string?  BatchNumber      { get; set; }
    public DateTime? ExpiryDate      { get; set; }
}

public class PurchaseListDto
{
    public int      Id             { get; set; }
    public string   PurchaseNumber { get; set; } = string.Empty;
    public DateTime PurchaseDate   { get; set; }
    public string   SupplierName   { get; set; } = string.Empty;
    public decimal  TotalAmount    { get; set; }
    public string   Status         { get; set; } = string.Empty;
    public int      ItemCount      { get; set; }
}
