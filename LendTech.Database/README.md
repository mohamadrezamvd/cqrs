# LendTech.Database

این لایه شامل موجودیت‌ها، DbContext و تنظیمات Entity Framework Core است.

## ساختار پروژه

```
LendTech.Database/
├── Entities/           # موجودیت‌های دیتابیس
├── Configurations/     # تنظیمات Fluent API
├── Extensions/         # Extension Methods
├── Helpers/           # کلاس‌های کمکی
└── LendTechDbContext.cs
```

## نحوه استفاده از Scaffold-DbContext

برای به‌روزرسانی موجودیت‌ها از دیتابیس:

```bash
Scaffold-DbContext "Server=localhost;Database=LendTech;Trusted_Connection=true;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Entities -Context LendTechDbContext -ContextDir . -Force
```

## ویژگی‌های کلیدی

1. **Multi-Tenant Support**: فیلترهای خودکار بر اساس OrganizationId
2. **Soft Delete**: حذف منطقی برای موجودیت‌های SoftDeletableEntity
3. **Audit Trail**: ثبت خودکار تغییرات در جدول AuditLogs
4. **Currency Conversion**: متد GetConvertedAmount برای تبدیل ارز

## Migration Commands

```bash
# ایجاد Migration جدید
Add-Migration InitialCreate -Project LendTech.Database -StartupProject LendTech.API

# اعمال Migration
Update-Database -Project LendTech.Database -StartupProject LendTech.API
```
