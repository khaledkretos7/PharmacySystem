namespace PharmacySystem.Application.DTOs.Expense;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreateExpenseCategoryDto
{
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateExpenseCategoryDto : CreateExpenseCategoryDto
{
    public int  Id       { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateExpenseDto
{
    public string   Title              { get; set; } = string.Empty;
    public string?  Description        { get; set; }
    public decimal  Amount             { get; set; }
    public DateTime ExpenseDate        { get; set; } = DateTime.UtcNow;
    public string?  Reference          { get; set; }
    public bool     IsRecurring        { get; set; } = false;
    public string?  Notes              { get; set; }
    public int      ExpenseCategoryId  { get; set; }
}

public class UpdateExpenseDto : CreateExpenseDto
{
    public int Id { get; set; }
}

// ── Responses ───────────────────────────────────────────────────────────────

public class ExpenseCategoryResponseDto
{
    public int     Id           { get; set; }
    public string  Name         { get; set; } = string.Empty;
    public string? Description  { get; set; }
    public bool    IsActive     { get; set; }
    public int     ExpenseCount { get; set; }
    public decimal TotalAmount  { get; set; }
}

public class ExpenseResponseDto
{
    public int      Id               { get; set; }
    public string   Title            { get; set; } = string.Empty;
    public string?  Description      { get; set; }
    public decimal  Amount           { get; set; }
    public DateTime ExpenseDate      { get; set; }
    public string?  Reference        { get; set; }
    public bool     IsRecurring      { get; set; }
    public string?  Notes            { get; set; }
    public int      ExpenseCategoryId{ get; set; }
    public string   CategoryName     { get; set; } = string.Empty;
    public string   RecordedByUser   { get; set; } = string.Empty;
    public DateTime CreatedAt        { get; set; }
}

public class ExpenseListDto
{
    public int      Id           { get; set; }
    public string   Title        { get; set; } = string.Empty;
    public decimal  Amount       { get; set; }
    public DateTime ExpenseDate  { get; set; }
    public string   CategoryName { get; set; } = string.Empty;
    public string?  Reference    { get; set; }
}
