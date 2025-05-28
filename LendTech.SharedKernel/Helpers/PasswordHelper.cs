using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using LendTech.SharedKernel.Constants;

namespace LendTech.SharedKernel.Helpers;

/// <summary>
/// کلاس کمکی برای کار با رمز عبور
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// هش کردن رمز عبور
    /// </summary>
    public static string HashPassword(string password)
    {
        // تولید Salt تصادفی
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // هش کردن با PBKDF2
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        // ترکیب salt و hash
        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    /// <summary>
    /// بررسی رمز عبور
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == hashed;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// اعتبارسنجی قدرت رمز عبور
    /// </summary>
    public static (bool IsValid, string Message) ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, ValidationMessages.Required.Replace("{0}", "رمز عبور"));

        if (password.Length < SecurityConstants.MinPasswordLength)
            return (false, ValidationMessages.PasswordTooShort.Replace("{0}", SecurityConstants.MinPasswordLength.ToString()));

        if (SecurityConstants.RequireUppercase && !Regex.IsMatch(password, "[A-Z]"))
            return (false, ValidationMessages.PasswordRequiresUppercase);

        if (SecurityConstants.RequireLowercase && !Regex.IsMatch(password, "[a-z]"))
            return (false, ValidationMessages.PasswordRequiresLowercase);

        if (SecurityConstants.RequireDigit && !Regex.IsMatch(password, "[0-9]"))
            return (false, ValidationMessages.PasswordRequiresDigit);

        if (SecurityConstants.RequireSpecialChar && !Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]"))
            return (false, ValidationMessages.PasswordRequiresSpecialChar);

        return (true, "رمز عبور معتبر است");
    }

    /// <summary>
    /// تولید رمز عبور تصادفی
    /// </summary>
    public static string GenerateRandomPassword(int length = 12)
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()";

        var password = new StringBuilder();
        var random = new Random();

        // اطمینان از وجود حداقل یک کاراکتر از هر نوع
        password.Append(lowercase[random.Next(lowercase.Length)]);
        password.Append(uppercase[random.Next(uppercase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(special[random.Next(special.Length)]);

        // پر کردن بقیه رمز
        const string all = lowercase + uppercase + digits + special;
        for (int i = 4; i < length; i++)
        {
            password.Append(all[random.Next(all.Length)]);
        }

        // مخلوط کردن کاراکترها
        return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
    }
}
