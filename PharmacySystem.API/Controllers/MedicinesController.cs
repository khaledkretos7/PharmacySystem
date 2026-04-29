using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Medicine;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Medicine (drug) management — CRUD, search, stock alerts.</summary>
[Authorize]
public class MedicinesController : BaseApiController
{
    private readonly IMedicineService _service;
    public MedicinesController(IMedicineService service) => _service = service;

    /// <summary>Get paged list of medicines with optional filters.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MedicineListDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] MedicineFilterDto filter, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(filter, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get a single medicine by Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Look up a medicine by its barcode.</summary>
    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode, CancellationToken ct)
    {
        var result = await _service.GetByBarcodeAsync(barcode, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Search medicines by name, generic name, or barcode.</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        var result = await _service.SearchAsync(q, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get all medicines below their reorder level.</summary>
    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock(CancellationToken ct)
    {
        var result = await _service.GetLowStockAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get medicines expiring within the specified number of days (default 30).</summary>
    [HttpGet("near-expiry")]
    public async Task<IActionResult> NearExpiry([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var result = await _service.GetNearExpiryAsync(days, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get all expired medicines.</summary>
    [HttpGet("expired")]
    public async Task<IActionResult> Expired(CancellationToken ct)
    {
        var result = await _service.GetExpiredAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get medicines by category.</summary>
    [HttpGet("by-category/{categoryId:int}")]
    public async Task<IActionResult> ByCategory(int categoryId, CancellationToken ct)
    {
        var result = await _service.GetByCategoryAsync(categoryId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Create a new medicine. [Admin only]</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<MedicineResponseDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Update an existing medicine. [Admin only]</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicineDto dto, CancellationToken ct)
    {
        dto.Id = id;
        var result = await _service.UpdateAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Soft-delete a medicine. [Admin only]</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }
}
