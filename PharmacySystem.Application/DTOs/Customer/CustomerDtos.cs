namespace PharmacySystem.Application.DTOs.Customer;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreateCustomerDto
{
    public string   Name              { get; set; } = string.Empty;
    public string?  Phone             { get; set; }
    public string?  Email             { get; set; }
    public string?  Address           { get; set; }
    public DateTime? DateOfBirth      { get; set; }
    public string?  Gender            { get; set; }
    public string?  InsuranceNumber   { get; set; }
    public string?  InsuranceProvider { get; set; }
    public string?  Allergies         { get; set; }
}

public class UpdateCustomerDto : CreateCustomerDto
{
    public int  Id       { get; set; }
    public bool IsActive { get; set; } = true;
}

// ── Responses ───────────────────────────────────────────────────────────────

public class CustomerResponseDto
{
    public int       Id                { get; set; }
    public string    Name              { get; set; } = string.Empty;
    public string?   Phone             { get; set; }
    public string?   Email             { get; set; }
    public string?   Address           { get; set; }
    public DateTime? DateOfBirth       { get; set; }
    public string?   Gender            { get; set; }
    public string?   InsuranceNumber   { get; set; }
    public string?   InsuranceProvider { get; set; }
    public string?   Allergies         { get; set; }
    public int       LoyaltyPoints     { get; set; }
    public bool      IsActive          { get; set; }
    public int       TotalInvoices     { get; set; }
    public decimal   TotalSpent        { get; set; }
    public DateTime  CreatedAt         { get; set; }
}

public class CustomerListDto
{
    public int     Id            { get; set; }
    public string  Name          { get; set; } = string.Empty;
    public string? Phone         { get; set; }
    public int     LoyaltyPoints { get; set; }
    public int     TotalInvoices { get; set; }
    public decimal TotalSpent    { get; set; }
}
