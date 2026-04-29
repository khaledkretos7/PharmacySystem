namespace PharmacySystem.Application.Common;

/// <summary>Pagination parameters passed in from the controller query string.</summary>
public class PagedRequest
{
    private int _pageSize = 20;
    private int _pageNumber = 1;

    /// <summary>1-based page number.</summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>Items per page (max 100).</summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 100 ? 100 : value < 1 ? 1 : value;
    }

    public string? SearchTerm  { get; set; }
    public string? SortBy      { get; set; }
    public bool    SortDesc    { get; set; } = false;
}
