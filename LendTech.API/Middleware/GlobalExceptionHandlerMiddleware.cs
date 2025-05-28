using System.Net;
using System.Text.Json;
using LendTech.SharedKernel.Exceptions;
using LendTech.SharedKernel.Models;
namespace LendTech.API.Middleware;
/// <summary>
/// Middleware مدیریت خطاهای سراسری
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
private readonly RequestDelegate _next;
private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
private readonly IWebHostEnvironment _environment;
public GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IWebHostEnvironment environment)
{
    _next = next;
    _logger = logger;
    _environment = environment;
}

public async Task InvokeAsync(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (Exception ex)
    {
        await HandleExceptionAsync(context, ex);
    }
}

private async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    var response = context.Response;
    response.ContentType = "application/json";

    var apiResponse = new ApiResponse();
    
    switch (exception)
    {
        case ValidationException validationEx:
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            apiResponse = ApiResponse.ValidationError(validationEx.Errors);
            _logger.LogWarning("خطای اعتبارسنجی: {Errors}", validationEx.ToString());
            break;

        case NotFoundException notFoundEx:
            response.StatusCode = (int)HttpStatusCode.NotFound;
            apiResponse = ApiResponse.NotFound(notFoundEx.Message);
            _logger.LogWarning("موجودیت یافت نشد: {EntityName} با شناسه {EntityId}", 
                notFoundEx.EntityName, notFoundEx.EntityId);
            break;

        case UnauthorizedException unauthorizedEx:
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            apiResponse = ApiResponse.Unauthorized(unauthorizedEx.Message);
            _logger.LogWarning("عدم احراز هویت: {Message}", unauthorizedEx.Message);
            break;

        case ForbiddenException forbiddenEx:
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            apiResponse = ApiResponse.Forbidden(forbiddenEx.Message);
            _logger.LogWarning("عدم دسترسی: {Operation} به {Resource}", 
                forbiddenEx.Operation, forbiddenEx.Resource);
            break;

        case BusinessException businessEx:
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            apiResponse = ApiResponse.Error(
                SharedKernel.Enums.ResponseStatus.BusinessError, 
                businessEx.Message,
                businessEx.Details);
            _logger.LogWarning("خطای تجاری: {ErrorCode} - {Message}", 
                businessEx.ErrorCode, businessEx.Message);
            break;

        //case RateLimitExceededException rateLimitEx:
        //    response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        //    response.Headers.Add("Retry-After", rateLimitEx.RetryAfterSeconds.ToString());
        //    apiResponse = ApiResponse.Error(
        //        SharedKernel.Enums.ResponseStatus.ServiceUnavailable,
        //        rateLimitEx.Message);
        //    _logger.LogWarning("تجاوز از حد مجاز درخواست: {Message}", rateLimitEx.Message);
        //    break;

        default:
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var message = _environment.IsDevelopment() 
                ? exception.ToString() 
                : "خطای داخلی سرور رخ داده است";
                
            apiResponse = ApiResponse.Error(
                SharedKernel.Enums.ResponseStatus.InternalServerError,
                message);
                
            _logger.LogError(exception, "خطای ناشناخته رخ داده است");
            break;
    }

    // اضافه کردن TraceId
    apiResponse.TraceId = context.TraceIdentifier;

    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    var result = JsonSerializer.Serialize(apiResponse, jsonOptions);
    await response.WriteAsync(result);
}
}
/// <summary>
/// Extension method برای اضافه کردن Middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
{
return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}
}
