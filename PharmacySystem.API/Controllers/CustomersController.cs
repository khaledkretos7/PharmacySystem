using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Customer;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Customer management.</summary>
[Authorize]
public class CustomersController : BaseApiController
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) => _service = service;

    /// <summary>Get paged list of customers.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _service.GetPagedAsync(request, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get a customer by Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Find a customer by phone number.</summary>
    [HttpGet("phone/{phone}")]
    public async Task<IActionResult> GetByPhone(string phone, CancellationToken ct)
    {
        var result = await _service.GetByPhoneAsync(phone, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get top customers by total spend.</summary>
    [HttpGet("top")]
    public async Task<IActionResult> TopCustomers([FromQuery] int count = 10, CancellationToken ct = default)
    {
        var result = await _service.GetTopCustomersAsync(count, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Create a new customer.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Update a customer.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Cashier")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto, CancellationToken ct)
    {
        dto.Id = id;
        var result = await _service.UpdateAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Soft-delete a customer. [Admin only]</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }
}
