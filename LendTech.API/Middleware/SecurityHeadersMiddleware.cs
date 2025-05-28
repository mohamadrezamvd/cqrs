namespace LendTech.API.Middleware;
/// <summary>
/// Middleware برای اضافه کردن Security Headers
/// </summary>
public class SecurityHeadersMiddleware
{
private readonly RequestDelegate _next;
public SecurityHeadersMiddleware(RequestDelegate next)
{
    _next = next;
}

public async Task InvokeAsync(HttpContext context)
{
    // Security Headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
    
    // Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' https://api.lendtech.com");
    
    // Feature Policy
    context.Response.Headers.Add("Permissions-Policy", 
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

    // HSTS (در Production)
    if (!context.Request.Host.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }

    await _next(context);
}
}
/// <summary>
/// Extension method برای اضافه کردن Middleware
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
{
return builder.UseMiddleware<SecurityHeadersMiddleware>();
}
}
