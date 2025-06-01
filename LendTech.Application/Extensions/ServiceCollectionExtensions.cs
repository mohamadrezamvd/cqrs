using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using System.Reflection;
using LendTech.Application.Behaviors;
using LendTech.Application.Services;
using LendTech.Application.Services.Interfaces;
using LendTech.Infrastructure.Repositories;
using LendTech.Infrastructure.Repositories.Interfaces;
using MediatR.NotificationPublishers;

namespace LendTech.Application.Extensions;
/// <summary>
/// Extension متدهای DI Container
/// </summary>
public static class ServiceCollectionExtensions
{
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
}
