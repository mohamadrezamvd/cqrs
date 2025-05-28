using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LendTech.SharedKernel.Extensions;
namespace LendTech.Application.Behaviors;
/// <summary>
/// Behavior لاگ‌گیری برای MediatR
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
{
    _logger = logger;
}

public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
{
    var requestName = typeof(TRequest).Name;
    var requestGuid = Guid.NewGuid().ToString();

    _logger.LogInformation("شروع پردازش درخواست {RequestName} {@RequestGuid} {@Request}",
        requestName, requestGuid, request);

    var stopwatch = Stopwatch.StartNew();

    try
    {
        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning("درخواست {RequestName} {@RequestGuid} بیش از 500ms طول کشید: {ElapsedMilliseconds}ms",
                requestName, requestGuid, stopwatch.ElapsedMilliseconds);
        }

        _logger.LogInformation("پایان پردازش درخواست {RequestName} {@RequestGuid} در {ElapsedMilliseconds}ms",
            requestName, requestGuid, stopwatch.ElapsedMilliseconds);

        return response;
    }
    catch (Exception ex)
    {
        stopwatch.Stop();

        _logger.LogError(ex, "خطا در پردازش درخواست {RequestName} {@RequestGuid} پس از {ElapsedMilliseconds}ms",
            requestName, requestGuid, stopwatch.ElapsedMilliseconds);

        throw;
    }
}
}
