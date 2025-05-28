using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace LendTech.API.Extensions;
/// <summary>
/// Extension methods برای API Versioning
/// </summary>
public static class VersioningExtensions
{
	/// <summary>
	/// اضافه کردن API Versioning
	/// </summary>
	public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
	{
		services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
			options.ApiVersionReader = ApiVersionReader.Combine(
				new UrlSegmentApiVersionReader(),
				new HeaderApiVersionReader("X-Api-Version"),
				new MediaTypeApiVersionReader("version"));
		});
		// API Explorer را جداگانه اضافه می‌کنیم
		services.AddVersionedApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});

		return services;
	}
}