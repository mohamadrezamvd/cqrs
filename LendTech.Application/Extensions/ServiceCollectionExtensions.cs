using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using System.Reflection;
using LendTech.Application.Behaviors;
using LendTech.Application.Services;
using LendTech.Application.Services.Interfaces;
<<<<<<< HEAD
=======
using LendTech.Infrastructure.Repositories;
using LendTech.Infrastructure.Repositories.Interfaces;
>>>>>>> 94aac394eddb4eac9d4131a1722b51b996524932
using MediatR.NotificationPublishers;

namespace LendTech.Application.Extensions;
/// <summary>
/// Extension متدهای DI Container
/// </summary>
public static class ServiceCollectionExtensions
{
<<<<<<< HEAD
/// <summary>
/// اضافه کردن سرویس‌های لایه Application
/// </summary>
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
var assembly = Assembly.GetExecutingAssembly();
    // MediatR
    services.AddMediatR(cfg => 
    {
        cfg.RegisterServicesFromAssembly(assembly);
        cfg.NotificationPublisher = new TaskWhenAllPublisher();
    });
=======
	/// <summary>
	/// اضافه کردن سرویس‌های لایه Application
	/// </summary>
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		var assembly = Assembly.GetExecutingAssembly();
		// MediatR
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(assembly);
			cfg.NotificationPublisher = new TaskWhenAllPublisher();
		});
>>>>>>> 94aac394eddb4eac9d4131a1722b51b996524932

		// AutoMapper
		services.AddAutoMapper(assembly);

		// FluentValidation
		services.AddValidatorsFromAssembly(assembly);

		// Behaviors
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

		// Business Services
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IPermissionService, PermissionService>();

<<<<<<< HEAD
    // Domain Event Handlers (اگر داریم)
    // services.AddScoped<INotificationHandler<UserCreatedEvent>, UserCreatedEventHandler>();

    return services;
}

/// <summary>
/// تنظیمات Validator ها
/// </summary>
public static IServiceCollection AddValidationRules(this IServiceCollection services)
{
    // می‌توانیم قوانین اعتبارسنجی سفارشی اضافه کنیم
    ValidatorOptions.Global.LanguageManager.Enabled = true;
    ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo("fa-IR");

    return services;
}
=======
		//Token
		services.AddScoped<IUserTokenRepository, UserTokenRepository>();
		// Domain Event Handlers (اگر داریم)
		// services.AddScoped<INotificationHandler<UserCreatedEvent>, UserCreatedEventHandler>();

		return services;
	}

	/// <summary>
	/// تنظیمات Validator ها
	/// </summary>
	public static IServiceCollection AddValidationRules(this IServiceCollection services)
	{
		// می‌توانیم قوانین اعتبارسنجی سفارشی اضافه کنیم
		ValidatorOptions.Global.LanguageManager.Enabled = true;
		ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo("fa-IR");

		return services;
	}
>>>>>>> 94aac394eddb4eac9d4131a1722b51b996524932
}
