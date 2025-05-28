using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
namespace LendTech.API.Extensions;
/// <summary>
/// Extension methods برای Rate Limiting
/// </summary>
public static class RateLimitingExtensions
{
/// <summary>
/// اضافه کردن Rate Limiting
/// </summary>
public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
{
services.AddRateLimiter(options =>
{
options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
RateLimitPartition.GetFixedWindowLimiter(
partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
factory: partition => new FixedWindowRateLimiterOptions
{
AutoReplenishment = true,
PermitLimit = 100,
QueueLimit = 0,
Window = TimeSpan.FromMinutes(1)
}));
        // Policy برای IP
        options.AddPolicy("PerIpPolicy", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 60,
                    QueueLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Policy برای کاربران احراز هویت شده
        options.AddPolicy("PerUserPolicy", httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? "anonymous",
                factory: partition => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 200,
                    QueueLimit = 20,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 4
                }));

        // Policy برای API های حساس
        options.AddPolicy("SensitiveApiPolicy", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 10,
                    QueueLimit = 2,
                    Window = TimeSpan.FromMinutes(5)
                }));

        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                await context.HttpContext.Response.WriteAsync(
                    $"تعداد درخواست‌ها بیش از حد مجاز است. لطفاً پس از {retryAfter.TotalSeconds} ثانیه مجدداً تلاش کنید.",
                    cancellationToken: token);
            }
            else
            {
                await context.HttpContext.Response.WriteAsync(
                    "تعداد درخواست‌ها بیش از حد مجاز است. لطفاً کمی صبر کنید.",
                    cancellationToken: token);
            }
        };
    });

    return services;
}
}
