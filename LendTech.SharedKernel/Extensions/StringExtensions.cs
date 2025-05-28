using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LendTech.SharedKernel.Extensions;

/// <summary>
/// Extension متدهای رشته
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// بررسی خالی یا null بودن رشته
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// بررسی خالی، null یا فقط شامل فاصله بودن رشته
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// تبدیل به CamelCase
    /// </summary>
    public static string ToCamelCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    /// <summary>
    /// تبدیل به PascalCase
    /// </summary>
    public static string ToPascalCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    /// <summary>
    /// تبدیل به Snake_Case
    /// </summary>
    public static string ToSnakeCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        
        var pattern = @"([a-z0-9])([A-Z])";
        return Regex.Replace(value, pattern, "$1_$2").ToLower();
    }

    /// <summary>
    /// تبدیل به Kebab-Case
    /// </summary>
    public static string ToKebabCase(this string value)
    {
        if (value.IsNullOrEmpty()) return value;
        
        var pattern = @"([a-z0-9])([A-Z])";
        return Regex.Replace(value, pattern, "$1-$2").ToLower();
    }

    /// <summary>
    /// حذف فاصله‌های اضافی
    /// </summary>
    public static string? TrimAll(this string? value)
    {
        if (value.IsNullOrEmpty()) return value;
        return Regex.Replace(value.Trim(), @"\s+", " ");
    }

    /// <summary>
    /// تبدیل اعداد انگلیسی به فارسی
    /// </summary>
    public static string ToPersianNumbers(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        var persianNumbers = new[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        for (int i = 0; i < 10; i++)
        {
            value = value.Replace(i.ToString(), persianNumbers[i]);
        }
        return value;
    }

    /// <summary>
    /// تبدیل اعداد فارسی به انگلیسی
    /// </summary>
    public static string ToEnglishNumbers(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        var persianNumbers = new[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        var arabicNumbers = new[] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
        
        for (int i = 0; i < 10; i++)
        {
            value = value.Replace(persianNumbers[i], i.ToString());
            value = value.Replace(arabicNumbers[i], i.ToString());
        }
        return value;
    }

    /// <summary>
    /// بررسی معتبر بودن ایمیل
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (email.IsNullOrWhiteSpace()) return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// بررسی معتبر بودن شماره موبایل ایران
    /// </summary>
    public static bool IsValidIranianMobile(this string mobile)
    {
        if (mobile.IsNullOrWhiteSpace()) return false;
        
        mobile = mobile.ToEnglishNumbers().Trim();
        var pattern = @"^(?:0|98|\+98)?9\d{9}$";
        return Regex.IsMatch(mobile, pattern);
    }

    /// <summary>
    /// نرمال‌سازی شماره موبایل ایران
    /// </summary>
    public static string? NormalizeIranianMobile(this string? mobile)
    {
        if (mobile.IsNullOrWhiteSpace()) return null;
        
        mobile = mobile.ToEnglishNumbers().Trim();
        mobile = Regex.Replace(mobile, @"[^\d]", "");
        
        if (mobile.StartsWith("98")) mobile = mobile.Substring(2);
        if (mobile.StartsWith("0")) mobile = mobile.Substring(1);
        
        return mobile.Length == 10 && mobile.StartsWith("9") ? mobile : null;
    }

    /// <summary>
    /// هش کردن رشته با SHA256
    /// </summary>
    public static string ToSha256(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// محدود کردن طول رشته
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (value.IsNullOrEmpty() || value.Length <= maxLength) return value;
        
        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// تولید Slug از رشته
    /// </summary>
    public static string ToSlug(this string value)
    {
        if (value.IsNullOrEmpty()) return value;

        // حروف فارسی را حفظ می‌کنیم
        value = value.ToLower().Trim();
        value = Regex.Replace(value, @"[^\u0600-\u06FF\w\s-]", ""); // حذف کاراکترهای غیرمجاز
        value = Regex.Replace(value, @"\s+", "-"); // تبدیل فاصله به خط تیره
        value = Regex.Replace(value, @"-+", "-"); // حذف خط تیره‌های اضافی
        
        return value.Trim('-');
    }
}
