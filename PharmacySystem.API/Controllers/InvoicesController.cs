using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Invoice;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>
/// Sales invoice management — creation (with auto stock deduction),
/// returns (with auto stock restoration), and cancellation.
/// </summary>
[Authorize]
public class InvoicesController : BaseApiController
{
    private readonly IInvoiceService _service;
    public InvoicesController(IInvoiceService service) => _service = service;

    /// <summary>Get paged list of invoices.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(request, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get an invoice by Id (includes all line items and returns).</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get an invoice by its invoice number.</summary>
    [HttpGet("number/{invoiceNumber}")]
    public async Task<IActionResult> GetByNumber(string invoiceNumber, CancellationToken ct)
    {
        var result = await _service.GetByNumberAsync(invoiceNumber, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get all invoices for a specific customer.</summary>
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer(int customerId, CancellationToken ct)
    {
        var result = await _service.GetByCustomerAsync(customerId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get invoices within a date range.</summary>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var result = await _service.GetByDateRangeAsync(from, to, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Create a confirmed sales invoice.
    /// Validates stock, blocks expired medicines, deducts stock, writes inventory audit — all in one transaction.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Cancel an invoice. [Admin only]</summary>
    [HttpPatch("{id:int}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await _service.CancelAsync(id, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Create a return for an invoice line item.
    /// Validates quantity against original, restores stock, writes inventory audit.
    /// </summary>
    [HttpPost("{id:int}/returns")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> CreateReturn(
        int id,
        [FromBody] CreateInvoiceReturnDto dto,
        CancellationToken ct)
    {
        dto.InvoiceId = id;
        var result = await _service.CreateReturnAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }
}
