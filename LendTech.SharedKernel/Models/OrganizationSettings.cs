using System;
using System.Collections.Generic;

namespace LendTech.SharedKernel.Models;

/// <summary>
/// مدل تنظیمات سازمان
/// </summary>
public class OrganizationSettings
{
    /// <summary>
    /// تنظیمات عمومی
    /// </summary>
    public GeneralSettings General { get; set; } = new();

    /// <summary>
    /// تنظیمات امنیتی
    /// </summary>
    public SecuritySettings Security { get; set; } = new();

    /// <summary>
    /// تنظیمات مالی
    /// </summary>
    public FinancialSettings Financial { get; set; } = new();

    /// <summary>
    /// تنظیمات نوتیفیکیشن
    /// </summary>
    public NotificationSettings Notification { get; set; } = new();

    /// <summary>
    /// Feature Flags
    /// </summary>
    public List<FeatureFlag> Features { get; set; } = new();
}

/// <summary>
/// تنظیمات عمومی
/// </summary>
public class GeneralSettings
{
    public string TimeZone { get; set; } = "Asia/Tehran";
    public string DateFormat { get; set; } = "yyyy/MM/dd";
    public string Language { get; set; } = "fa-IR";
    public string Theme { get; set; } = "default";
}

/// <summary>
/// تنظیمات امنیتی
/// </summary>
public class SecuritySettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 30;
    public int PasswordExpirationDays { get; set; } = 90;
    public bool RequireTwoFactor { get; set; } = false;
    public List<string> AllowedIpAddresses { get; set; } = new();
}

/// <summary>
/// تنظیمات مالی
/// </summary>
public class FinancialSettings
{
    public string DefaultCurrency { get; set; } = "IRR";
    public decimal MaxTransactionAmount { get; set; } = 1000000000;
    public decimal MinTransactionAmount { get; set; } = 10000;
    public int TransactionTimeout { get; set; } = 300; // ثانیه
}

/// <summary>
/// تنظیمات نوتیفیکیشن
/// </summary>
public class NotificationSettings
{
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = false;
    public Dictionary<string, bool> EventSubscriptions { get; set; } = new();
}
