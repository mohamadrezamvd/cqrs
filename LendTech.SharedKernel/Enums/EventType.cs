namespace LendTech.SharedKernel.Enums;

/// <summary>
/// انواع رویدادهای سیستم برای Messaging
/// </summary>
public enum EventType
{
    // رویدادهای کاربر
    UserCreated,
    UserUpdated,
    UserDeleted,
    UserActivated,
    UserDeactivated,
    UserPasswordChanged,
    UserLocked,
    UserUnlocked,

    // رویدادهای نقش
    RoleCreated,
    RoleUpdated,
    RoleDeleted,
    RoleAssignedToUser,
    RoleRemovedFromUser,

    // رویدادهای سازمان
    OrganizationCreated,
    OrganizationUpdated,
    OrganizationActivated,
    OrganizationDeactivated,

    // رویدادهای مالی
    LoanRequested,
    LoanApproved,
    LoanRejected,
    LoanDisbursed,
    PaymentReceived,
    PaymentOverdue,

    // رویدادهای سیستمی
    SystemMaintenanceScheduled,
    SystemBackupCompleted,
    SystemErrorOccurred
}
