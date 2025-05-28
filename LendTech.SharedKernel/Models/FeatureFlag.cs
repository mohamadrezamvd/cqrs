using System;
using System.Collections.Generic;

namespace LendTech.SharedKernel.Models;

/// <summary>
/// مدل Feature Flag
/// </summary>
public class FeatureFlag
{
    /// <summary>
    /// نام Feature
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// وضعیت فعال بودن
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// پارامترهای Feature
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// تاریخ شروع
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// تاریخ پایان
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// بررسی فعال بودن در تاریخ مشخص
    /// </summary>
    public bool IsActiveAt(DateTime date)
    {
        if (!IsEnabled) return false;

        if (StartDate.HasValue && date < StartDate.Value) return false;
        
        if (EndDate.HasValue && date > EndDate.Value) return false;

        return true;
    }
}
