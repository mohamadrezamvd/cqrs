namespace LendTech.BackgroundTasks.Options;
/// <summary>
/// تنظیمات سرویس Outbox
/// </summary>
public class OutboxOptions
{
/// <summary>
/// فاصله زمانی پردازش (ثانیه)
/// </summary>
public int ProcessIntervalSeconds { get; set; } = 10;
/// <summary>
/// تعداد رکورد در هر بچ
/// </summary>
public int BatchSize { get; set; } = 100;

/// <summary>
/// حداکثر تعداد تلاش مجدد
/// </summary>
public int MaxRetryCount { get; set; } = 3;
}
