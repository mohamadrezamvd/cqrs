using Microsoft.AspNetCore.Authorization;
using LendTech.Application.Services.Interfaces;
using LendTech.SharedKernel.Constants;
namespace LendTech.API.Filters;
/// <summary>
/// Handler برای بررسی دسترسی‌ها
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
private readonly IServiceProvider _serviceProvider;
private readonly ILogger<PermissionAuthorizationHandler> _logger;
public PermissionAuthorizationHandler(
    IServiceProvider serviceProvider,
    ILogger<PermissionAuthorizationHandler> logger)
{
    _serviceProvider = serviceProvider;
    _logger = logger;
}

protected override async Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    PermissionRequirement requirement)
{
    var userIdClaim = context.User.FindFirst(SecurityConstants.Claims.UserId);
    
    if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
    {
        _logger.LogWarning("کاربر فاقد شناسه معتبر است");
        context.Fail();
        return;
    }

    // استفاده از ServiceProvider برای دریافت IPermissionService به صورت Scoped
    using var scope = _serviceProvider.CreateScope();
    var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

    var hasPermission = await permissionService.HasPermissionAsync(userId, requirement.Permission);

    if (hasPermission)
    {
        context.Succeed(requirement);
        _logger.LogDebug("کاربر {UserId} دسترسی {Permission} را دارد", userId, requirement.Permission);
    }
    else
    {
        context.Fail();
        _logger.LogWarning("کاربر {UserId} دسترسی {Permission} را ندارد", userId, requirement.Permission);
    }
}
}
/// <summary>
/// Requirement برای دسترسی
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
public string Permission { get; }
public PermissionRequirement(string permission)
{
    Permission = permission;
}
}
