using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.DTOs.Auth;

// ── Requests ────────────────────────────────────────────────────────────────

/// <summary>Login credentials.</summary>
public class LoginDto
{
    /// <example>admin@pharmacy.com</example>
    public string Email    { get; set; } = string.Empty;
    /// <example>Admin@1234</example>
    public string Password { get; set; } = string.Empty;
}

/// <summary>New user registration payload.</summary>
public class RegisterDto
{
    public string   FullName { get; set; } = string.Empty;
    public string   Email    { get; set; } = string.Empty;
    public string   Password { get; set; } = string.Empty;
    public string   ConfirmPassword { get; set; } = string.Empty;
    public UserRole Role     { get; set; } = UserRole.Cashier;
}

/// <summary>Refresh access token using a valid refresh token.</summary>
public class RefreshTokenDto
{
    public string AccessToken  { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>Change password request.</summary>
public class ChangePasswordDto
{
    public string UserId          { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword     { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>Update role of an existing user.</summary>
public class UpdateUserRoleDto
{
    public string UserId { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

// ── Responses ───────────────────────────────────────────────────────────────

/// <summary>Auth response returned after successful login/register/refresh.</summary>
public class AuthResponseDto
{
    public string  AccessToken        { get; set; } = string.Empty;
    public string  RefreshToken       { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry  { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public UserDto  User              { get; set; } = null!;
}

/// <summary>Slim user profile for embedding in auth responses and user lists.</summary>
public class UserDto
{
    public string Id       { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string Role     { get; set; } = string.Empty;
    public bool   IsActive { get; set; }
}
