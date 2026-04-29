using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Expense;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Expense and expense category management.</summary>
[Authorize(Roles = "Admin")]
public class ExpensesController : BaseApiController
{
    private readonly IExpenseService _service;
    public ExpensesController(IExpenseService service) => _service = service;

    // ── Expenses ──────────────────────────────────────────────────────────────

    /// <summary>Get paged list of expenses.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(request, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get an expense by Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get expenses within a date range.</summary>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var result = await _service.GetByDateRangeAsync(from, to, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Create a new expense.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Update an expense.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto, CancellationToken ct)
    {
        dto.Id = id;
        var result = await _service.UpdateAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Delete an expense.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    // ── Expense Categories ────────────────────────────────────────────────────

    /// <summary>Get all expense categories.</summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var result = await _service.GetCategoriesAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Create a new expense category.</summary>
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateExpenseCategoryDto dto, CancellationToken ct)
    {
        var result = await _service.CreateCategoryAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Update an expense category.</summary>
    [HttpPut("categories/{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateExpenseCategoryDto dto, CancellationToken ct)
    {
        dto.Id = id;
        var result = await _service.UpdateCategoryAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Delete an expense category (fails if expenses exist).</summary>
    [HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct)
    {
        var result = await _service.DeleteCategoryAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }
}
