using LendTech.API.Filters;
using LendTech.API.Policies;
using LendTech.Application.Services.Interfaces;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace LendTech.API.Extensions;
/// <summary>
/// Extension methods برای Authentication و Authorization
/// </summary>
public static class AuthenticationExtensions
{
	/// <summary>
	/// اضافه کردن JWT Authentication
	/// </summary>
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.SaveToken = true;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = configuration["Jwt:Issuer"],
				ValidAudience = configuration["Jwt:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
				ClockSkew = TimeSpan.Zero
			};

			options.Events = new JwtBearerEvents
			{
				OnTokenValidated = async context =>
				{
					try
					{
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

						// دریافت توکن از هدر Authorization
						var token = context.HttpContext.Request.Headers["Authorization"]
							.FirstOrDefault()?.Split(" ").Last();

						if (string.IsNullOrEmpty(token))
						{
							logger.LogWarning(
								"Authentication failed: Token not found. IP: {IpAddress}, Path: {Path}",
								context.HttpContext.Connection.RemoteIpAddress,
								context.HttpContext.Request.Path);

							context.Fail("توکن یافت نشد");
							return;
						}

						// بررسی اعتبار توکن در دیتابیس
						var principal = await tokenService.ValidateTokenAsync(token);
						if (principal == null)
						{
							logger.LogWarning(
								"Authentication failed: Invalid token. IP: {IpAddress}, Path: {Path}",
								context.HttpContext.Connection.RemoteIpAddress,
								context.HttpContext.Request.Path);

							context.Fail("توکن نامعتبر است");
							return;
						}

						// لاگ موفقیت
						logger.LogInformation(
							"Authentication successful. User: {UserId}, IP: {IpAddress}, Path: {Path}",
							principal.FindFirst(SecurityConstants.Claims.UserId)?.Value,
							context.HttpContext.Connection.RemoteIpAddress,
							context.HttpContext.Request.Path);

						// جایگزینی Claims با اطلاعات به‌روز از توکن معتبر
						context.Principal = principal;
					}
					catch (Exception ex)
					{
						var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
						logger.LogError(
							ex,
							"Authentication error. IP: {IpAddress}, Path: {Path}, Error: {Error}",
							context.HttpContext.Connection.RemoteIpAddress,
							context.HttpContext.Request.Path,
							ex.Message);

						context.Fail($"خطا در اعتبارسنجی توکن: {ex.Message}");
					}
				},

				OnAuthenticationFailed = context =>
				{
					var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

					if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
					{
						context.Response.Headers.Add("Token-Expired", "true");
						logger.LogWarning(
							"Authentication failed: Token expired. IP: {IpAddress}, Path: {Path}",
							context.HttpContext.Connection.RemoteIpAddress,
							context.HttpContext.Request.Path);
					}
					else
					{
						logger.LogError(
							context.Exception,
							"Authentication failed. IP: {IpAddress}, Path: {Path}, Error: {Error}",
							context.HttpContext.Connection.RemoteIpAddress,
							context.HttpContext.Request.Path,
							context.Exception.Message);
					}

					return Task.CompletedTask;
				},

				OnChallenge = async context =>
				{
					var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

					if (context.AuthenticateFailure != null)
					{
						context.HandleResponse();
						context.Response.StatusCode = 401;
						context.Response.ContentType = "application/json";

						var message = context.AuthenticateFailure is SecurityTokenExpiredException
							? "توکن منقضی شده است"
							: "توکن نامعتبر است";

						logger.LogWarning(
							"Authentication challenge. IP: {IpAddress}, Path: {Path}, Message: {Message}",
							context.HttpContext.Connection.RemoteIpAddress,
							context.HttpContext.Request.Path,
							message);

						var response = ApiResponse.Unauthorized(message);
						await context.Response.WriteAsJsonAsync(response);
					}
					return;
				}
			};
		});

		return services;
	}

	/// <summary>
	/// اضافه کردن Permission-based Authorization
	/// </summary>
	public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
	{
		// تغییر از Singleton به Scoped
		services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

		services.AddAuthorization(options =>
		{
			// Default Policy
			options.DefaultPolicy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build();

			// User Management Policies
			options.AddPolicy(PolicyNames.ViewUsers, policy =>
				policy.Requirements.Add(new PermissionRequirement("Users.View")));

			options.AddPolicy(PolicyNames.CreateUser, policy =>
				policy.Requirements.Add(new PermissionRequirement("Users.Create")));

			options.AddPolicy(PolicyNames.UpdateUser, policy =>
				policy.Requirements.Add(new PermissionRequirement("Users.Update")));

			options.AddPolicy(PolicyNames.DeleteUser, policy =>
				policy.Requirements.Add(new PermissionRequirement("Users.Delete")));

			// Role Management Policies
			options.AddPolicy(PolicyNames.ViewRoles, policy =>
				policy.Requirements.Add(new PermissionRequirement("Roles.View")));

			options.AddPolicy(PolicyNames.CreateRole, policy =>
				policy.Requirements.Add(new PermissionRequirement("Roles.Create")));

			options.AddPolicy(PolicyNames.UpdateRole, policy =>
				policy.Requirements.Add(new PermissionRequirement("Roles.Update")));

			options.AddPolicy(PolicyNames.DeleteRole, policy =>
				policy.Requirements.Add(new PermissionRequirement("Roles.Delete")));

			// Permission Management Policies
			options.AddPolicy(PolicyNames.ViewPermissions, policy =>
				policy.Requirements.Add(new PermissionRequirement("Permissions.View")));

			options.AddPolicy(PolicyNames.ManagePermissions, policy =>
				policy.Requirements.Add(new PermissionRequirement("Permissions.Manage")));

			// Organization Management Policies
			options.AddPolicy(PolicyNames.ViewOrganization, policy =>
				policy.Requirements.Add(new PermissionRequirement("Organization.View")));

			options.AddPolicy(PolicyNames.UpdateOrganization, policy =>
				policy.Requirements.Add(new PermissionRequirement("Organization.Update")));

			options.AddPolicy(PolicyNames.ManageOrganizationSettings, policy =>
				policy.Requirements.Add(new PermissionRequirement("Organization.ManageSettings")));

			// Financial Policies
			options.AddPolicy(PolicyNames.ViewFinancialReports, policy =>
				policy.Requirements.Add(new PermissionRequirement("Financial.ViewReports")));

			options.AddPolicy(PolicyNames.ManageLoans, policy =>
				policy.Requirements.Add(new PermissionRequirement("Loans.Manage")));

			options.AddPolicy(PolicyNames.ApproveLoans, policy =>
				policy.Requirements.Add(new PermissionRequirement("Loans.Approve")));
		});

		return services;
	}
}
