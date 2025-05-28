using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using LendTech.API.Filters;
using LendTech.API.Policies;
using LendTech.SharedKernel.Constants;
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
var jwtSettings = configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
var key = Encoding.UTF8.GetBytes(secretKey!);
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // می‌توانیم اینجا بررسی‌های اضافی انجام دهیم
                return Task.CompletedTask;
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
