using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای مواردی که موجودیت یافت نشد
/// </summary>
public class NotFoundException : BusinessException
{
    /// <summary>
    /// نام موجودیت
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// شناسه موجودیت
    /// </summary>
    public object? EntityId { get; }

    public NotFoundException(string entityName, object? entityId = null)
        : base($"{entityName} یافت نشد", "NOT_FOUND")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string entityName, object entityId, string message)
        : base(message, "NOT_FOUND")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
