using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Auth;

namespace PharmacySystem.Application.Interfaces;

/// <summary>Authentication and user-management contract.</summary>
public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>            RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<ApiResponse<UserDto>>         GetProfileAsync(string userId, CancellationToken ct = default);
    Task<ApiResponse<bool>>            ChangePasswordAsync(ChangePasswordDto dto, CancellationToken ct = default);
    Task<ApiResponse<List<UserDto>>>   GetAllUsersAsync(CancellationToken ct = default);
    Task<ApiResponse<bool>>            UpdateUserRoleAsync(UpdateUserRoleDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>            DeactivateUserAsync(string userId, CancellationToken ct = default);
}

/// <summary>JWT + refresh-token generation contract (implemented in Infrastructure).</summary>
public interface ITokenService
{
    string  GenerateAccessToken(string userId, string email, string role);
    string  GenerateRefreshToken();
    DateTime GetAccessTokenExpiry();
    DateTime GetRefreshTokenExpiry();
    string? GetUserIdFromExpiredToken(string token);
}
