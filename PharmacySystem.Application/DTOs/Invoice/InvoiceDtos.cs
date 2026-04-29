using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.DTOs.Invoice;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreateInvoiceDto
{
    public int?           CustomerId             { get; set; }
    public PaymentMethod  PaymentMethod          { get; set; } = PaymentMethod.Cash;
    public decimal        GlobalDiscountAmount   { get; set; } = 0;
    public decimal        AmountPaid             { get; set; }
    public string?        PrescriptionReference  { get; set; }
    public string?        Notes                  { get; set; }
    public List<CreateInvoiceItemDto> Items      { get; set; } = [];
}

public class CreateInvoiceItemDto
{
    public int     MedicineId    { get; set; }
    public int     Quantity      { get; set; }
    /// <summary>Line-level discount rate 0–1 (e.g. 0.10 = 10%).</summary>
    public decimal DiscountRate  { get; set; } = 0;
    /// <summary>Tax rate 0–1 (e.g. 0.15 = 15%). Defaults to system tax.</summary>
    public decimal TaxRate       { get; set; } = 0;
}

public class CreateInvoiceReturnDto
{
    public int          InvoiceId  { get; set; }
    public int          MedicineId { get; set; }
    public int          Quantity   { get; set; }
    public ReturnReason Reason     { get; set; } = ReturnReason.Other;
    public string?      Notes      { get; set; }
}

// ── Responses ───────────────────────────────────────────────────────────────

public class InvoiceResponseDto
{
    public int                          Id                    { get; set; }
    public string                       InvoiceNumber         { get; set; } = string.Empty;
    public DateTime                     InvoiceDate           { get; set; }
    public int?                         CustomerId            { get; set; }
    public string?                      CustomerName          { get; set; }
    public string                       PaymentMethod         { get; set; } = string.Empty;
    public string                       Status                { get; set; } = string.Empty;
    public decimal                      SubTotal              { get; set; }
    public decimal                      DiscountAmount        { get; set; }
    public decimal                      TaxAmount             { get; set; }
    public decimal                      TotalAmount           { get; set; }
    public decimal                      AmountPaid            { get; set; }
    public decimal                      ChangeAmount          { get; set; }
    public string?                      PrescriptionReference { get; set; }
    public string?                      Notes                 { get; set; }
    public string                       CashierName           { get; set; } = string.Empty;
    public List<InvoiceItemResponseDto> Items                 { get; set; } = [];
    public List<InvoiceReturnResponseDto> Returns             { get; set; } = [];
    public DateTime                     CreatedAt             { get; set; }
}

public class InvoiceItemResponseDto
{
    public int      Id             { get; set; }
    public int      MedicineId     { get; set; }
    public string   MedicineName   { get; set; } = string.Empty;
    public string   GenericName    { get; set; } = string.Empty;
    public string   Unit           { get; set; } = string.Empty;
    public int      Quantity       { get; set; }
    public decimal  UnitPrice      { get; set; }
    public decimal  DiscountRate   { get; set; }
    public decimal  DiscountAmount { get; set; }
    public decimal  TaxRate        { get; set; }
    public decimal  TaxAmount      { get; set; }
    public decimal  TotalAmount    { get; set; }
    public string?  BatchNumber    { get; set; }
    public DateTime? ExpiryDate    { get; set; }
}

public class InvoiceReturnResponseDto
{
    public int      Id             { get; set; }
    public string   ReturnNumber   { get; set; } = string.Empty;
    public int      MedicineId     { get; set; }
    public string   MedicineName   { get; set; } = string.Empty;
    public int      Quantity       { get; set; }
    public decimal  RefundAmount   { get; set; }
    public string   Reason         { get; set; } = string.Empty;
    public string?  Notes          { get; set; }
    public bool     IsRefunded     { get; set; }
    public DateTime ReturnDate     { get; set; }
}

/// <summary>Lightweight invoice for lists and reports.</summary>
public class InvoiceListDto
{
    public int      Id            { get; set; }
    public string   InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate   { get; set; }
    public string?  CustomerName  { get; set; }
    public string   PaymentMethod { get; set; } = string.Empty;
    public decimal  TotalAmount   { get; set; }
    public string   Status        { get; set; } = string.Empty;
    public int      ItemCount     { get; set; }
    public string   CashierName   { get; set; } = string.Empty;
}
