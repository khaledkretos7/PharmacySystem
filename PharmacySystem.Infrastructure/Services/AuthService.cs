using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Auth;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Infrastructure.Identity;

namespace PharmacySystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService               _tokenService;
    private readonly ILogger<AuthService>        _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager  = userManager;
        _tokenService = tokenService;
        _logger       = logger;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles        = await _userManager.GetRolesAsync(user);
        var role         = roles.FirstOrDefault() ?? "Cashier";
        var accessToken  = _tokenService.GenerateAccessToken(user.Id, user.Email!, role);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken       = refreshToken;
        user.RefreshTokenExpiry = _tokenService.GetRefreshTokenExpiry();
        user.UpdatedAt          = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            AccessToken        = accessToken,
            RefreshToken       = refreshToken,
            AccessTokenExpiry  = _tokenService.GetAccessTokenExpiry(),
            RefreshTokenExpiry = user.RefreshTokenExpiry.Value,
            User = new UserDto
            {
                Id       = user.Id,
                FullName = user.FullName,
                Email    = user.Email!,
                Role     = role,
                IsActive = user.IsActive
            }
        };
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null || !user.IsActive)
            return ApiResponse<AuthResponseDto>.Unauthorized("Invalid email or password.");

        if (await _userManager.IsLockedOutAsync(user))
            return ApiResponse<AuthResponseDto>.Unauthorized("Account is locked. Try again later.");

        var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValid)
        {
            await _userManager.AccessFailedAsync(user);
            return ApiResponse<AuthResponseDto>.Unauthorized("Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        _logger.LogInformation("User {Email} logged in.", user.Email);

        return ApiResponse<AuthResponseDto>.Ok(await BuildAuthResponseAsync(user));
    }

    // ── Register ──────────────────────────────────────────────────────────────

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var exists = await _userManager.FindByEmailAsync(dto.Email);
        if (exists is not null)
            return ApiResponse<AuthResponseDto>.Fail("Email is already registered.", 409);

        var user = new ApplicationUser
        {
            FullName       = dto.FullName,
            UserName       = dto.Email,
            Email          = dto.Email,
            EmailConfirmed = true,
            IsActive       = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return ApiResponse<AuthResponseDto>.Fail("Registration failed.", 400, errors);
        }

        await _userManager.AddToRoleAsync(user, dto.Role.ToString());
        _logger.LogInformation("User {Email} registered with role {Role}.", user.Email, dto.Role);

        return ApiResponse<AuthResponseDto>.Created(await BuildAuthResponseAsync(user));
    }

    // ── Refresh Token ─────────────────────────────────────────────────────────

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct = default)
    {
        var userId = _tokenService.GetUserIdFromExpiredToken(dto.AccessToken);
        if (userId is null)
            return ApiResponse<AuthResponseDto>.Unauthorized("Invalid access token.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || user.RefreshToken != dto.RefreshToken)
            return ApiResponse<AuthResponseDto>.Unauthorized("Invalid refresh token.");

        if (user.RefreshTokenExpiry <= DateTime.UtcNow)
            return ApiResponse<AuthResponseDto>.Unauthorized("Refresh token has expired. Please login again.");

        return ApiResponse<AuthResponseDto>.Ok(await BuildAuthResponseAsync(user));
    }

    // ── Revoke Token ──────────────────────────────────────────────────────────

    public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user is null) return ApiResponse<bool>.NotFound("Token not found.");

        user.RefreshToken       = null;
        user.RefreshTokenExpiry = null;
        await _userManager.UpdateAsync(user);
        return ApiResponse<bool>.Ok(true, "Token revoked.");
    }

    // ── Profile ───────────────────────────────────────────────────────────────

    public async Task<ApiResponse<UserDto>> GetProfileAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return ApiResponse<UserDto>.NotFound("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponse<UserDto>.Ok(new UserDto
        {
            Id       = user.Id,
            FullName = user.FullName,
            Email    = user.Email!,
            Role     = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        });
    }

    // ── Change Password ───────────────────────────────────────────────────────

    public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null) return ApiResponse<bool>.NotFound("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return ApiResponse<bool>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

        // Revoke refresh token after password change (force re-login)
        user.RefreshToken       = null;
        user.RefreshTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        return ApiResponse<bool>.Ok(true, "Password changed successfully.");
    }

    // ── All Users (Admin) ─────────────────────────────────────────────────────

    public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = _userManager.Users.ToList();
        var dtos  = new List<UserDto>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            dtos.Add(new UserDto
            {
                Id       = u.Id,
                FullName = u.FullName,
                Email    = u.Email!,
                Role     = roles.FirstOrDefault() ?? string.Empty,
                IsActive = u.IsActive
            });
        }
        return ApiResponse<List<UserDto>>.Ok(dtos);
    }

    // ── Update Role ───────────────────────────────────────────────────────────

    public async Task<ApiResponse<bool>> UpdateUserRoleAsync(UpdateUserRoleDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null) return ApiResponse<bool>.NotFound("User not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role.ToString());

        return ApiResponse<bool>.Ok(true, $"Role updated to {dto.Role}.");
    }

    // ── Deactivate User ───────────────────────────────────────────────────────

    public async Task<ApiResponse<bool>> DeactivateUserAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return ApiResponse<bool>.NotFound("User not found.");

        user.IsActive           = false;
        user.RefreshToken       = null;
        user.RefreshTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        return ApiResponse<bool>.Ok(true, "User deactivated.");
    }
}
