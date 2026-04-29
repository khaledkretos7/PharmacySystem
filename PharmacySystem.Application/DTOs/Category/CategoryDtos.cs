namespace PharmacySystem.Application.DTOs.Category;

// ── Requests ────────────────────────────────────────────────────────────────

public class CreateCategoryDto
{
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCategoryDto : CreateCategoryDto
{
    public int  Id       { get; set; }
    public bool IsActive { get; set; } = true;
}

// ── Responses ───────────────────────────────────────────────────────────────

public class CategoryResponseDto
{
    public int      Id            { get; set; }
    public string   Name          { get; set; } = string.Empty;
    public string?  Description   { get; set; }
    public bool     IsActive      { get; set; }
    public int      MedicineCount { get; set; }
    public DateTime CreatedAt     { get; set; }
}
