using System;

namespace LendTech.SharedKernel.Exceptions;

/// <summary>
/// Exception برای عدم دسترسی
/// </summary>
public class ForbiddenException : BusinessException
{
    /// <summary>
    /// عملیات درخواستی
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// منبع درخواستی
    /// </summary>
    public string? Resource { get; }

    public ForbiddenException(string message = "شما به این بخش دسترسی ندارید")
        : base(message, "FORBIDDEN")
    {
    }

    public ForbiddenException(string operation, string resource)
        : base($"شما دسترسی {operation} به {resource} را ندارید", "FORBIDDEN")
    {
        Operation = operation;
        Resource = resource;
    }
}
