namespace LendTech.SharedKernel.Constants;

/// <summary>
/// ثابت‌های امنیتی
/// </summary>
public static class SecurityConstants
{
    /// <summary>
    /// نام Scheme احراز هویت
    /// </summary>
    public const string AuthenticationScheme = "Bearer";

    /// <summary>
    /// نام Policy پیش‌فرض
    /// </summary>
    public const string DefaultPolicy = "DefaultPolicy";

    /// <summary>
    /// Claims
    /// </summary>
    public static class Claims
    {
        public const string UserId = "userId";
        public const string Username = "username";
        public const string Email = "email";
        public const string OrganizationId = "organizationId";
        public const string OrganizationCode = "organizationCode";
        public const string Roles = "roles";
        public const string Permissions = "permissions";
    }

    /// <summary>
    /// Headers
    /// </summary>
    public static class Headers
    {
        public const string Authorization = "Authorization";
        public const string ApiKey = "X-API-Key";
        public const string RequestId = "X-Request-Id";
        public const string TenantId = "X-Tenant-Id";
        public const string ClientIp = "X-Forwarded-For";
        public const string UserAgent = "User-Agent";
    }

    /// <summary>
    /// طول حداقل رمز عبور
    /// </summary>
    public const int MinPasswordLength = 8;

    /// <summary>
    /// الزام به داشتن حروف بزرگ در رمز عبور
    /// </summary>
    public const bool RequireUppercase = true;

    /// <summary>
    /// الزام به داشتن حروف کوچک در رمز عبور
    /// </summary>
    public const bool RequireLowercase = true;

    /// <summary>
    /// الزام به داشتن عدد در رمز عبور
    /// </summary>
    public const bool RequireDigit = true;

    /// <summary>
    /// الزام به داشتن کاراکتر خاص در رمز عبور
    /// </summary>
    public const bool RequireSpecialChar = true;
}
