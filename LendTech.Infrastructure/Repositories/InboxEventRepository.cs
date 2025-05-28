using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Extensions;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository رویدادهای Inbox
/// </summary>
public class InboxEventRepository : Repository<InboxEvent>, IInboxEventRepository
{
    public InboxEventRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<bool> IsProcessedAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(e => e.MessageId == messageId && e.ProcessedAt != null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InboxEvent> RecordEventAsync(string messageId, string eventType, object eventData, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var @event = new InboxEvent
        {
            MessageId = messageId,
            EventType = eventType,
            EventData = eventData.ToJson(),
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow
        };

        await AddAsync(@event, cancellationToken);
        return @event;
    }

    /// <inheritdoc />
    public async Task MarkAsProcessedAsync(string messageId, CancellationToken cancellationToken = default)
    {
        var @event = await _dbSet
            .FirstOrDefaultAsync(e => e.MessageId == messageId, cancellationToken);
            
        if (@event != null && @event.ProcessedAt == null)
        {
            @event.ProcessedAt = DateTime.UtcNow;
            await UpdateAsync(@event, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupOldEventsAsync(int daysToKeep, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        
        var eventsToDelete = await _dbSet
            .Where(e => e.ProcessedAt != null && e.ProcessedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(eventsToDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return eventsToDelete.Count;
    }
}
