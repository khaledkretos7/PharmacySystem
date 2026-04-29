using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) => _config = config;

    public string GenerateAccessToken(string userId, string email, string role)
    {
        var secretKey = _config["JWT:SecretKey"]
                        ?? throw new InvalidOperationException("JWT:SecretKey is missing.");

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email,          email),
            new(ClaimTypes.Role,           role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer:             _config["JWT:Issuer"],
            audience:           _config["JWT:Audience"],
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            GetAccessTokenExpiry(),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public DateTime GetAccessTokenExpiry()
        => DateTime.UtcNow.AddMinutes(
               double.Parse(_config["JWT:AccessTokenExpiryMinutes"] ?? "60"));

    public DateTime GetRefreshTokenExpiry()
        => DateTime.UtcNow.AddDays(
               double.Parse(_config["JWT:RefreshTokenExpiryDays"] ?? "7"));

    public string? GetUserIdFromExpiredToken(string token)
    {
        var secretKey = _config["JWT:SecretKey"] ?? string.Empty;

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = _config["JWT:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = _config["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime         = false   // allow expired tokens for refresh flow
        };

        try
        {
            var handler    = new JwtSecurityTokenHandler();
            var principal  = handler.ValidateToken(token, parameters, out _);
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        catch { return null; }
    }
}
