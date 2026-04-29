using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Purchase;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Purchase order management — create orders, receive goods (supports partial delivery).</summary>
[Authorize(Roles = "Admin")]
public class PurchasesController : BaseApiController
{
    private readonly IPurchaseService _service;
    public PurchasesController(IPurchaseService service) => _service = service;

    /// <summary>Get paged list of purchase orders.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(request, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get a purchase order by Id (includes all items).</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get all purchase orders for a supplier.</summary>
    [HttpGet("supplier/{supplierId:int}")]
    public async Task<IActionResult> GetBySupplier(int supplierId, CancellationToken ct)
    {
        var result = await _service.GetBySupplierAsync(supplierId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Create a new purchase order.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Mark goods as received.
    /// Updates stock, writes inventory audit. Supports partial delivery.
    /// </summary>
    [HttpPost("{id:int}/receive")]
    public async Task<IActionResult> Receive(int id, [FromBody] ReceivePurchaseDto dto, CancellationToken ct)
    {
        dto.PurchaseId = id;
        var result = await _service.ReceiveAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Cancel a purchase order (not allowed after receiving).</summary>
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await _service.CancelAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }
}
