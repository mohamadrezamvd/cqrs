using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository رویدادهای Outbox
/// </summary>
public interface IOutboxEventRepository : IRepository<OutboxEvent>
{
    /// <summary>
    /// دریافت رویدادهای پردازش نشده
    /// </summary>
    Task<List<OutboxEvent>> GetUnprocessedEventsAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت رویدادهای ناموفق
    /// </summary>
    Task<List<OutboxEvent>> GetFailedEventsAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// علامت‌گذاری به عنوان پردازش شده
    /// </summary>
    Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// علامت‌گذاری به عنوان ناموفق
    /// </summary>
    Task MarkAsFailedAsync(Guid eventId, string? errorMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزایش تعداد تلاش
    /// </summary>
    Task IncrementRetryCountAsync(Guid eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// پاکسازی رویدادهای قدیمی
    /// </summary>
    Task<int> CleanupOldEventsAsync(int daysToKeep, CancellationToken cancellationToken = default);

    /// <summary>
    /// ایجاد رویداد جدید
    /// </summary>
    Task<OutboxEvent> CreateEventAsync(string eventType, object eventData, Guid organizationId, CancellationToken cancellationToken = default);
}
