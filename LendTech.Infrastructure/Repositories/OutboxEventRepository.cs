using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LendTech.Database;
using LendTech.Database.Entities;
using LendTech.Infrastructure.Repositories.Interfaces;
using LendTech.SharedKernel.Extensions;

namespace LendTech.Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository رویدادهای Outbox
/// </summary>
public class OutboxEventRepository : Repository<OutboxEvent>, IOutboxEventRepository
{
    public OutboxEventRepository(LendTechDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<List<OutboxEvent>> GetUnprocessedEventsAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.ProcessedAt == null)
            .OrderBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<OutboxEvent>> GetFailedEventsAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.ProcessedAt == null && e.RetryCount < maxRetryCount)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await GetByIdAsync(eventId, cancellationToken);
        if (@event != null)
        {
            @event.ProcessedAt = DateTime.UtcNow;
            await UpdateAsync(@event, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task MarkAsFailedAsync(Guid eventId, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var @event = await GetByIdAsync(eventId, cancellationToken);
        if (@event != null)
        {
            @event.RetryCount++;
            
            // اضافه کردن پیام خطا به EventData
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var data = @event.EventData.FromJson<Dictionary<string, object>>() ?? new Dictionary<string, object>();
                data["LastError"] = errorMessage;
                data["LastErrorAt"] = DateTime.UtcNow;
                @event.EventData = data.ToJson();
            }
            
            await UpdateAsync(@event, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task IncrementRetryCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var @event = await GetByIdAsync(eventId, cancellationToken);
        if (@event != null)
        {
            @event.RetryCount++;
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

    /// <inheritdoc />
    public async Task<OutboxEvent> CreateEventAsync(string eventType, object eventData, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var @event = new OutboxEvent
        {
            EventType = eventType,
            EventData = eventData.ToJson(),
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow
        };

        await AddAsync(@event, cancellationToken);
        return @event;
    }
}
