# LendTech.SharedKernel

این لایه شامل کلاس‌ها، ابزارها و مدل‌های مشترک است که در تمام لایه‌های دیگر استفاده می‌شوند.

## ساختار پروژه

```
LendTech.SharedKernel/
├── Constants/        # ثابت‌های سیستم
├── Enums/           # Enum های مشترک
├── Exceptions/      # Exception های سفارشی
├── Extensions/      # Extension Methods
├── Helpers/         # کلاس‌های کمکی
└── Models/          # مدل‌های مشترک
```

## ویژگی‌های کلیدی

### Extensions
- **StringExtensions**: متدهای کمکی رشته (تبدیل اعداد فارسی/انگلیسی، اعتبارسنجی ایمیل و موبایل)
- **DateTimeExtensions**: کار با تاریخ شمسی و محاسبات زمانی
- **CollectionExtensions**: عملیات روی مجموعه‌ها
- **NumericExtensions**: فرمت‌بندی اعداد و محاسبات
- **JsonExtensions**: سریالایز/دیسریالایز JSON

### Helpers
- **PasswordHelper**: هش کردن و اعتبارسنجی رمز عبور
- **ValidationHelper**: اعتبارسنجی کد ملی، شبا، کارت بانکی
- **SecurityHelper**: رمزنگاری، تولید توکن

### Exceptions
- **BusinessException**: خطاهای منطق تجاری
- **ValidationException**: خطاهای اعتبارسنجی
- **NotFoundException**: موجودیت یافت نشد
- **UnauthorizedException**: عدم احراز هویت
- **ForbiddenException**: عدم دسترسی

### Models
- **ApiResponse**: مدل استاندارد پاسخ API
- **PagedResult**: نتایج صفحه‌بندی شده
- **IntegrationEvent**: رویدادهای Integration
- **FeatureFlag**: مدیریت Feature ها
- **OrganizationSettings**: تنظیمات سازمان

## نحوه استفاده

```csharp
// استفاده از Extensions
var persianDate = DateTime.Now.ToPersianDate();
var isValidMobile = "09123456789".IsValidIranianMobile();

// استفاده از Helpers
var hashedPassword = PasswordHelper.HashPassword("myPassword");
var isValidNationalCode = ValidationHelper.IsValidNationalCode("1234567890");

// استفاده از Models
var response = ApiResponse<User>.SuccessResult(user);
var pagedResult = new PagedResult<Product>(products, totalCount, pageNumber, pageSize);
```
