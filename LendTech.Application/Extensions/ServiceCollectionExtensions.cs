using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using System.Reflection;
using LendTech.Application.Behaviors;
using LendTech.Application.Services;
using LendTech.Application.Services.Interfaces;
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
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

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

    return services;
}
}
