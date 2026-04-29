using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Auth;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Authentication and user management endpoints.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    // ── Public endpoints ──────────────────────────────────────────────────────

    /// <summary>Login and receive JWT access + refresh tokens.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Obtain a new access token using a valid refresh token.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Invalidate a refresh token (logout from current device).</summary>
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] string refreshToken, CancellationToken ct)
    {
        var result = await _authService.RevokeTokenAsync(refreshToken, ct);
        return StatusCode(result.StatusCode, result);
    }

    // ── Authenticated user endpoints ──────────────────────────────────────────

    /// <summary>Get the profile of the currently authenticated user.</summary>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _authService.GetProfileAsync(userId, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Change password for the currently authenticated user.</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken ct)
    {
        dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _authService.ChangePasswordAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    // ── Admin-only endpoints ──────────────────────────────────────────────────

    /// <summary>Register a new user. [Admin only]</summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 201)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>List all users. [Admin only]</summary>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), 200)]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await _authService.GetAllUsersAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Update a user's role. [Admin only]</summary>
    [HttpPut("users/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateUserRoleDto dto, CancellationToken ct)
    {
        var result = await _authService.UpdateUserRoleAsync(dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Deactivate a user account. [Admin only]</summary>
    [HttpPatch("users/{userId}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(string userId, CancellationToken ct)
    {
        var result = await _authService.DeactivateUserAsync(userId, ct);
        return StatusCode(result.StatusCode, result);
    }
}
