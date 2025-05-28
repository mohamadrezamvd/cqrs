using System;

namespace LendTech.SharedKernel.Extensions;

/// <summary>
/// Extension متدهای عددی
/// </summary>
public static class NumericExtensions
{
    /// <summary>
    /// بررسی عدد بودن در محدوده
    /// </summary>
    public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    /// محدود کردن عدد در بازه
    /// </summary>
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    /// <summary>
    /// تبدیل به مبلغ با جداکننده هزارگان
    /// </summary>
    public static string ToMoney(this decimal value, string currency = "ریال")
    {
        return $"{value:N0} {currency}";
    }

    /// <summary>
    /// تبدیل به مبلغ با جداکننده هزارگان
    /// </summary>
    public static string ToMoney(this int value, string currency = "ریال")
    {
        return $"{value:N0} {currency}";
    }

    /// <summary>
    /// تبدیل به مبلغ با جداکننده هزارگان
    /// </summary>
    public static string ToMoney(this long value, string currency = "ریال")
    {
        return $"{value:N0} {currency}";
    }

    /// <summary>
    /// تبدیل به درصد
    /// </summary>
    public static string ToPercentage(this decimal value, int decimals = 2)
    {
        return $"{value.ToString($"F{decimals}")}%";
    }

    /// <summary>
    /// تبدیل به درصد
    /// </summary>
    public static string ToPercentage(this double value, int decimals = 2)
    {
        return $"{value.ToString($"F{decimals}")}%";
    }

    /// <summary>
    /// تبدیل بایت به واحد خوانا
    /// </summary>
    public static string ToFileSize(this long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// گرد کردن به تعداد رقم اعشار مشخص
    /// </summary>
    public static decimal RoundTo(this decimal value, int decimals)
    {
        return Math.Round(value, decimals);
    }

    /// <summary>
    /// گرد کردن به بالا
    /// </summary>
    public static int CeilToInt(this decimal value)
    {
        return (int)Math.Ceiling(value);
    }

    /// <summary>
    /// گرد کردن به پایین
    /// </summary>
    public static int FloorToInt(this decimal value)
    {
        return (int)Math.Floor(value);
    }

    /// <summary>
    /// محاسبه درصد از کل
    /// </summary>
    public static decimal PercentageOf(this decimal part, decimal total)
    {
        if (total == 0) return 0;
        return (part / total) * 100;
    }

    /// <summary>
    /// محاسبه مقدار از درصد
    /// </summary>
    public static decimal PercentOf(this decimal percentage, decimal total)
    {
        return (percentage / 100) * total;
    }

    /// <summary>
    /// بررسی زوج بودن
    /// </summary>
    public static bool IsEven(this int value)
    {
        return value % 2 == 0;
    }

    /// <summary>
    /// بررسی فرد بودن
    /// </summary>
    public static bool IsOdd(this int value)
    {
        return value % 2 != 0;
    }

    /// <summary>
    /// تبدیل عدد به حروف فارسی
    /// </summary>
    public static string ToWords(this int number)
    {
        if (number == 0) return "صفر";
        
        if (number < 0) return "منفی " + Math.Abs(number).ToWords();
        
        var ones = new[] { "", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        var tens = new[] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        var hundreds = new[] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        var special = new[] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };

        if (number < 10) return ones[number];
        if (number < 20) return special[number - 10];
        if (number < 100) return tens[number / 10] + (number % 10 > 0 ? " و " + ones[number % 10] : "");
        if (number < 1000) return hundreds[number / 100] + (number % 100 > 0 ? " و " + (number % 100).ToWords() : "");
        if (number < 1000000) return (number / 1000).ToWords() + " هزار" + (number % 1000 > 0 ? " و " + (number % 1000).ToWords() : "");
        
        return number.ToString();
    }
}
