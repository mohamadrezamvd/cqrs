using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LendTech.Infrastructure.Repositories.Interfaces;

namespace LendTech.BackgroundTasks.Services;

/// <summary>
/// سرویس پاکسازی توکن‌های منقضی شده
/// </summary>
public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(
        IServiceProvider serviceProvider,
        ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("شروع پاکسازی توکن‌های منقضی شده");

                using var scope = _serviceProvider.CreateScope();
                var userTokenRepository = scope.ServiceProvider.GetRequiredService<IUserTokenRepository>();

                await userTokenRepository.CleanupExpiredTokensAsync(stoppingToken);

                _logger.LogInformation("پاکسازی توکن‌های منقضی شده با موفقیت انجام شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پاکسازی توکن‌های منقضی شده");
            }

            // اجرای هر 12 ساعت
            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }
} 