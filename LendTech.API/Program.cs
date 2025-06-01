using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using LendTech.API.Extensions;
using LendTech.API.Middleware;
using LendTech.Application.Extensions;
using LendTech.BackgroundTasks.Extensions;
using LendTech.Database.SeedData;
using LendTech.Infrastructure.Extensions;
using LendTech.SharedKernel.Constants;
// تنظیم Serilog
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
.Enrich.FromLogContext()
.Enrich.WithEnvironmentName()
.Enrich.WithMachineName()
.WriteTo.Console()
.CreateBootstrapLogger();
try
{
	Log.Information("شروع راه‌اندازی سرویس LendTech API");
	var builder = WebApplication.CreateBuilder(args);

	// تنظیم Serilog
	builder.Host.UseSerilog((context, configuration) =>
	{
		configuration
			.ReadFrom.Configuration(context.Configuration)
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithEnvironmentName()
			.WriteTo.Console()
			.WriteTo.Seq(
				serverUrl: builder.Configuration["Seq:ServerUrl"]!,
				apiKey: builder.Configuration["Seq:ApiKey"]);
	});

	// افزودن سرویس‌ها
	ConfigureServices(builder.Services, builder.Configuration);

	var app = builder.Build();

	// مقداردهی اولیه دیتابیس
	await InitializeDatabase(app);

	// تنظیم Middleware Pipeline
	ConfigurePipeline(app);

	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "خطای بحرانی در راه‌اندازی سرویس");
}
finally
{
	Log.CloseAndFlush();
}
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
	// سرویس‌های لایه‌های مختلف
	services.AddInfrastructureServices(configuration);
	services.AddApplicationServices();
	services.AddBackgroundTaskServices(configuration);
	// API Services
	services.AddControllers();
	services.AddEndpointsApiExplorer();

	// Swagger
	services.AddSwaggerConfiguration();

	// Authentication & Authorization
	services.AddJwtAuthentication(configuration);
	services.AddPermissionBasedAuthorization();

	// CORS
	services.AddCorsConfiguration(configuration);

	// Health Checks
	services.AddHealthChecksConfiguration(configuration);

	// Rate Limiting
	services.AddRateLimiting(configuration);

	// Versioning
	services.AddApiVersioningConfiguration();

	// HttpContext Accessor
	services.AddHttpContextAccessor();

	// OpenTelemetry
	services.AddOpenTelemetry()
		.ConfigureResource(resource => resource
			.AddService(
				serviceName: SystemConstants.SystemName,
				serviceVersion: SystemConstants.ApiVersion))
		.WithTracing(tracing => tracing
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.AddSqlClientInstrumentation()
			.AddJaegerExporter(options =>
			{
				options.AgentHost = configuration["Jaeger:AgentHost"] ?? "localhost";
				options.AgentPort = int.Parse(configuration["Jaeger:AgentPort"] ?? "6831");
			}))
		.WithMetrics(metrics => metrics
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.AddPrometheusExporter());
}
void ConfigurePipeline(WebApplication app)
{
	// Development specific
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "LendTech API V1");
			c.RoutePrefix = string.Empty;
		});
	}
	// Security Headers
	app.UseSecurityHeaders();

	// HTTPS Redirection
	app.UseHttpsRedirection();

	// Serilog Request Logging
	app.UseSerilogRequestLogging(options =>
	{
		options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
		options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
		{
			diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
			diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
			diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
			diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
		};
	});

	// Exception Handling
	app.UseGlobalExceptionHandler();

	// CORS
	app.UseCors("LendTechPolicy");

	// Rate Limiting
	app.UseRateLimiter();

	// Health Checks
	app.MapHealthChecks("/health", new HealthCheckOptions
	{
		Predicate = _ => true,
		ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
	});

	app.MapHealthChecksUI(options =>
	{
		options.UIPath = "/health-ui";
	});

	// Prometheus Metrics
	app.UseOpenTelemetryPrometheusScrapingEndpoint();

	// Authentication & Authorization
	app.UseAuthentication();
	app.UseAuthorization();

	// Request Localization
	app.UseRequestLocalization(options =>
	{
		options.SetDefaultCulture("fa-IR");
		options.AddSupportedCultures("fa-IR", "en-US");
		options.AddSupportedUICultures("fa-IR", "en-US");
	});

	// Map Controllers
	app.MapControllers();
}
/// < summary >
/// مقداردهی اولیه دیتابیس
/// </summary>
async Task InitializeDatabase(WebApplication app)
{
	try
	{
		Log.Information("شروع مقداردهی اولیه دیتابیس...");
		using var scope = app.Services.CreateScope();
		await DatabaseInitializer.InitializeAsync(scope.ServiceProvider);

		Log.Information("مقداردهی اولیه دیتابیس با موفقیت انجام شد.");
	}
	catch (Exception ex)
	{
		Log.Fatal(ex, "خطا در مقداردهی اولیه دیتابیس");
		throw;
	}
}
