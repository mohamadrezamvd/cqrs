using System;
using System.Globalization;

namespace LendTech.SharedKernel.Extensions;

/// <summary>
/// Extension متدهای تاریخ و زمان
/// </summary>
public static class DateTimeExtensions
{
    private static readonly PersianCalendar PersianCalendar = new();

    /// <summary>
    /// تبدیل به تاریخ شمسی
    /// </summary>
    public static string ToPersianDate(this DateTime dateTime, string format = "yyyy/MM/dd")
    {
        var year = PersianCalendar.GetYear(dateTime);
        var month = PersianCalendar.GetMonth(dateTime);
        var day = PersianCalendar.GetDayOfMonth(dateTime);

        return format
            .Replace("yyyy", year.ToString("D4"))
            .Replace("yy", (year % 100).ToString("D2"))
            .Replace("MM", month.ToString("D2"))
            .Replace("M", month.ToString())
            .Replace("dd", day.ToString("D2"))
            .Replace("d", day.ToString());
    }

    /// <summary>
    /// تبدیل به تاریخ و زمان شمسی
    /// </summary>
    public static string ToPersianDateTime(this DateTime dateTime, string format = "yyyy/MM/dd HH:mm:ss")
    {
        var datePart = dateTime.ToPersianDate("yyyy/MM/dd");
        var timePart = dateTime.ToString("HH:mm:ss");
        
        return format
            .Replace("yyyy/MM/dd", datePart)
            .Replace("HH:mm:ss", timePart)
            .Replace("HH:mm", dateTime.ToString("HH:mm"));
    }

    /// <summary>
    /// تبدیل تاریخ شمسی به میلادی
    /// </summary>
    public static DateTime? FromPersianDate(this string persianDate)
    {
        if (string.IsNullOrWhiteSpace(persianDate)) return null;

        try
        {
            var parts = persianDate.Split('/', '-');
            if (parts.Length != 3) return null;

            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            var day = int.Parse(parts[2]);

            return PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// شروع روز
    /// </summary>
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// پایان روز
    /// </summary>
    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// شروع هفته
    /// </summary>
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Saturday)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-diff).Date;
    }

    /// <summary>
    /// پایان هفته
    /// </summary>
    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Saturday)
    {
        return dateTime.StartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
    }

    /// <summary>
    /// شروع ماه
    /// </summary>
    public static DateTime StartOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// پایان ماه
    /// </summary>
    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// شروع سال
    /// </summary>
    public static DateTime StartOfYear(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1);
    }

    /// <summary>
    /// پایان سال
    /// </summary>
    public static DateTime EndOfYear(this DateTime dateTime)
    {
        return dateTime.StartOfYear().AddYears(1).AddTicks(-1);
    }

    /// <summary>
    /// تبدیل به Unix Timestamp
    /// </summary>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    /// <summary>
    /// تبدیل از Unix Timestamp
    /// </summary>
    public static DateTime FromUnixTimestamp(this long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
    }

    /// <summary>
    /// محاسبه سن
    /// </summary>
    public static int CalculateAge(this DateTime birthDate, DateTime? currentDate = null)
    {
        var today = currentDate ?? DateTime.Today;
        var age = today.Year - birthDate.Year;
        
        if (birthDate.Date > today.AddYears(-age)) age--;
        
        return age;
    }

    /// <summary>
    /// بررسی روز کاری (غیر تعطیل)
    /// </summary>
    public static bool IsWorkingDay(this DateTime date)
    {
        // جمعه تعطیل است در ایران
        return date.DayOfWeek != DayOfWeek.Friday;
    }

    /// <summary>
    /// دریافت نام روز به فارسی
    /// </summary>
    public static string GetPersianDayName(this DateTime date)
    {
        var dayNames = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنج‌شنبه", "جمعه", "شنبه" };
        return dayNames[(int)date.DayOfWeek];
    }

    /// <summary>
    /// دریافت نام ماه شمسی
    /// </summary>
    public static string GetPersianMonthName(this DateTime date)
    {
        var monthNames = new[] 
        { 
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };
        
        var month = PersianCalendar.GetMonth(date);
        return monthNames[month - 1];
    }

    /// <summary>
    /// محاسبه اختلاف زمانی به صورت خوانا
    /// </summary>
    public static string ToRelativeTime(this DateTime dateTime, DateTime? baseTime = null)
    {
        var currentTime = baseTime ?? DateTime.Now;
        var timeSpan = currentTime - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return "لحظاتی پیش";

        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} دقیقه پیش";

        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} ساعت پیش";

        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} روز پیش";

        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} ماه پیش";

        return $"{(int)(timeSpan.TotalDays / 365)} سال پیش";
    }
}
