using System;
using System.Threading;
using System.Threading.Tasks;
using LendTech.Database.Entities;

namespace LendTech.Infrastructure.Repositories.Interfaces;

/// <summary>
/// اینترفیس Repository رویدادهای Inbox
/// </summary>
public interface IInboxEventRepository : IRepository<InboxEvent>
{
    /// <summary>
    /// بررسی پردازش شدن پیام
    /// </summary>
    Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ثبت پیام دریافتی
    /// </summary>
    Task<InboxEvent> RecordEventAsync(string messageId, string eventType, object eventData, Guid organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// علامت‌گذاری به عنوان پردازش شده
    /// </summary>
    Task MarkAsProcessedAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// پاکسازی رویدادهای قدیمی
    /// </summary>
    Task<int> CleanupOldEventsAsync(int daysToKeep, CancellationToken cancellationToken = default);
}
