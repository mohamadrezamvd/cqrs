using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace LendTech.Infrastructure.Security;
/// <summary>
/// کلاس کمکی برای کار با JWT
/// </summary>
public static class JwtHelper
{
private static readonly JwtSecurityTokenHandler TokenHandler = new();
/// <summary>
/// تولید توکن JWT
/// </summary>
public static string GenerateToken(
    Dictionary<string, string> claims,
    string secretKey,
    string issuer,
    string audience,
    int expirationHours)
{
    var key = Encoding.UTF8.GetBytes(secretKey);
    var claimsList = new List<Claim>();

    foreach (var claim in claims)
    {
        claimsList.Add(new Claim(claim.Key, claim.Value));
    }

    // اضافه کردن Claims استاندارد
    claimsList.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
    claimsList.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claimsList),
        Expires = DateTime.UtcNow.AddHours(expirationHours),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var token = TokenHandler.CreateToken(tokenDescriptor);
    return TokenHandler.WriteToken(token);
}

/// <summary>
/// اعتبارسنجی توکن
/// </summary>
public static ClaimsPrincipal? ValidateToken(
    string token,
    string secretKey,
    string issuer,
    string audience)
{
    try
    {
        var key = Encoding.UTF8.GetBytes(secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        var principal = TokenHandler.ValidateToken(token, validationParameters, out _);
        return principal;
    }
    catch
    {
        return null;
    }
}

/// <summary>
/// استخراج Claim از توکن
/// </summary>
public static string? GetClaimValue(ClaimsPrincipal principal, string claimType)
{
    return principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
}

/// <summary>
/// تولید Refresh Token
/// </summary>
public static string GenerateRefreshToken()
{
    var randomNumber = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
}
}
