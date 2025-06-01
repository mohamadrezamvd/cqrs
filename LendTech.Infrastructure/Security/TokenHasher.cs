using System;
using System.Security.Cryptography;
using System.Text;

namespace LendTech.Infrastructure.Security;

/// <summary>
/// کلاس کمکی برای هش کردن توکن‌ها
/// </summary>
public static class TokenHasher
{
    /// <summary>
    /// هش کردن توکن
    /// </summary>
    public static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// مقایسه توکن با هش
    /// </summary>
    public static bool VerifyToken(string token, string hash)
    {
        var computedHash = HashToken(token);
        return computedHash == hash;
    }
} 