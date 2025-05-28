using System;
using System.Security.Cryptography;
using System.Text;

namespace LendTech.SharedKernel.Helpers;

/// <summary>
/// کلاس کمکی برای عملیات امنیتی
/// </summary>
public static class SecurityHelper
{
    /// <summary>
    /// تولید توکن تصادفی
    /// </summary>
    public static string GenerateRandomToken(int length = 32)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// تولید کد تایید عددی
    /// </summary>
    public static string GenerateNumericCode(int length = 6)
    {
        var random = new Random();
        var code = "";
        
        for (int i = 0; i < length; i++)
        {
            code += random.Next(0, 10).ToString();
        }
        
        return code;
    }

    /// <summary>
    /// رمزنگاری متن با AES
    /// </summary>
    public static string Encrypt(string plainText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = GetKeyBytes(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[aes.IV.Length + cipherBytes.Length];
        aes.IV.CopyTo(result, 0);
        cipherBytes.CopyTo(result, aes.IV.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// رمزگشایی متن با AES
    /// </summary>
    public static string Decrypt(string cipherText, string key)
    {
        var cipherBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = GetKeyBytes(key);

        var iv = new byte[aes.IV.Length];
        var cipher = new byte[cipherBytes.Length - iv.Length];

        Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
        Array.Copy(cipherBytes, iv.Length, cipher, 0, cipher.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// تولید HMAC-SHA256
    /// </summary>
    public static string GenerateHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(GetKeyBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// مخفی کردن بخشی از رشته
    /// </summary>
    public static string MaskString(string value, int visibleStart = 3, int visibleEnd = 3, char maskChar = '*')
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.Length <= visibleStart + visibleEnd)
            return new string(maskChar, value.Length);

        var masked = value.Substring(0, visibleStart);
        masked += new string(maskChar, value.Length - visibleStart - visibleEnd);
        masked += value.Substring(value.Length - visibleEnd);

        return masked;
    }

    /// <summary>
    /// مخفی کردن ایمیل
    /// </summary>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        var localPart = parts[0];
        var domain = parts[1];

        if (localPart.Length <= 2)
            localPart = new string('*', localPart.Length);
        else
            localPart = localPart[0] + new string('*', localPart.Length - 2) + localPart[localPart.Length - 1];

        return $"{localPart}@{domain}";
    }

    /// <summary>
    /// تبدیل کلید به بایت با طول ثابت
    /// </summary>
    private static byte[] GetKeyBytes(string key)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }
}
