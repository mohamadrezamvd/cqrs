namespace LendTech.API.Policies;
/// <summary>
/// نام Policy های Authorization
/// </summary>
public static class PolicyNames
{
// User Management
public const string ViewUsers = "Policy.ViewUsers";
public const string CreateUser = "Policy.CreateUser";
public const string UpdateUser = "Policy.UpdateUser";
public const string DeleteUser = "Policy.DeleteUser";
// Role Management
public const string ViewRoles = "Policy.ViewRoles";
public const string CreateRole = "Policy.CreateRole";
public const string UpdateRole = "Policy.UpdateRole";
public const string DeleteRole = "Policy.DeleteRole";

// Permission Management
public const string ViewPermissions = "Policy.ViewPermissions";
public const string ManagePermissions = "Policy.ManagePermissions";

// Organization Management
public const string ViewOrganization = "Policy.ViewOrganization";
public const string UpdateOrganization = "Policy.UpdateOrganization";
public const string ManageOrganizationSettings = "Policy.ManageOrganizationSettings";

// Financial Operations
public const string ViewFinancialReports = "Policy.ViewFinancialReports";
public const string ManageLoans = "Policy.ManageLoans";
public const string ApproveLoans = "Policy.ApproveLoans";
}
