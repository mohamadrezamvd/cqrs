using HealthChecks.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
namespace LendTech.API.Extensions;
/// <summary>
/// Extension methods برای Health Checks
/// </summary>
public static class HealthCheckExtensions
{
/// <summary>
/// اضافه کردن Health Checks
/// </summary>
public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
{
	services.AddHealthChecks()
// SQL Server
		.AddSqlServer(
			connectionString: configuration.GetConnectionString("DefaultConnection")!,
			healthQuery: "SELECT 1;",
			name: "SQL Server",
			failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
			tags: new[] { "database", "sql" })
		// Redis
		.AddRedis(
			redisConnectionString: configuration.GetConnectionString("Redis")!,
			name: "Redis",
			failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
			tags: new[] { "cache", "redis" })

		// RabbitMQ
		.AddRabbitMQ(
			rabbitConnectionString:
			$"amqp://{configuration["RabbitMQ:UserName"]}:{configuration["RabbitMQ:Password"]}@{configuration["RabbitMQ:HostName"]}:{configuration["RabbitMQ:Port"]}/",
			name: "RabbitMQ",
			failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
			tags: new[] { "messaging", "rabbitmq" })
		.AddMongoDb(
			sp => new MongoDB.Driver.MongoClient(
				configuration.GetSection("Serilog")["WriteTo:1:Args:databaseUrl"] ??
				"mongodb://localhost:27017/LendTechLogs"
			),
			name: "MongoDB",
			failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
			tags: new[] { "database", "mongo" }
		);


		// Health Check UI
		services.AddHealthChecksUI(options =>
    {
        options.SetEvaluationTimeInSeconds(30); // هر 30 ثانیه
        options.MaximumHistoryEntriesPerEndpoint(100);
        options.SetApiMaxActiveRequests(1);

        options.AddHealthCheckEndpoint("LendTech API", "/health");
    })
    .AddInMemoryStorage();

    return services;
}
}
