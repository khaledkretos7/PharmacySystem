using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Inventory;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Inventory management — audit trail, stock adjustments, physical count.</summary>
[Authorize(Roles = "Admin")]
public class InventoryController : BaseApiController
{
    private readonly IInventoryService _service;
    public InventoryController(IInventoryService service) => _service = service;

    /// <summary>Get paged inventory audit trail.</summary>
    [HttpGet("audits")]
    public async Task<IActionResult> GetAudits([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _service.GetAuditsAsync(request, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get all audit entries for a specific medicine.</summary>
    [HttpGet("audits/medicine/{medicineId:int}")]
    public async Task<IActionResult> GetByMedicine(int medicineId, CancellationToken ct)
    {
        var result = await _service.GetByMedicineAsync(medicineId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Manually adjust stock for a medicine.
    /// Positive value = stock in, Negative value = stock out.
    /// Writes an InventoryAudit record automatically.
    /// </summary>
    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustStock([FromBody] StockAdjustmentDto dto, CancellationToken ct)
    {
        var result = await _service.AdjustStockAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Run a physical inventory count.
    /// Compares actual counts against system quantities,
    /// adjusts discrepancies, and returns variance report.
    /// </summary>
    [HttpPost("physical-count")]
    public async Task<IActionResult> RunPhysicalCount([FromBody] PhysicalCountDto dto, CancellationToken ct)
    {
        var result = await _service.RunPhysicalCountAsync(dto, CurrentUserId, ct);
        return StatusCode(result.StatusCode, result);
    }
}
