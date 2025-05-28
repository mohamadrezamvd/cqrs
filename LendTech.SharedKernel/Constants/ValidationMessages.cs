namespace LendTech.SharedKernel.Constants;

/// <summary>
/// پیام‌های اعتبارسنجی
/// </summary>
public static class ValidationMessages
{
    // پیام‌های عمومی
    public const string Required = "فیلد {0} الزامی است";
    public const string MaxLength = "طول فیلد {0} نباید بیشتر از {1} کاراکتر باشد";
    public const string MinLength = "طول فیلد {0} نباید کمتر از {1} کاراکتر باشد";
    public const string Range = "مقدار فیلد {0} باید بین {1} و {2} باشد";
    public const string Email = "آدرس ایمیل معتبر نیست";
    public const string Phone = "شماره تلفن معتبر نیست";
    public const string Duplicate = "{0} تکراری است";
    public const string NotFound = "{0} یافت نشد";
    public const string Invalid = "{0} نامعتبر است";

    // پیام‌های رمز عبور
    public const string PasswordTooShort = "رمز عبور باید حداقل {0} کاراکتر باشد";
    public const string PasswordRequiresUppercase = "رمز عبور باید حداقل یک حرف بزرگ داشته باشد";
    public const string PasswordRequiresLowercase = "رمز عبور باید حداقل یک حرف کوچک داشته باشد";
    public const string PasswordRequiresDigit = "رمز عبور باید حداقل یک عدد داشته باشد";
    public const string PasswordRequiresSpecialChar = "رمز عبور باید حداقل یک کاراکتر خاص داشته باشد";
    public const string PasswordMismatch = "رمز عبور و تکرار آن یکسان نیستند";

    // پیام‌های احراز هویت
    public const string InvalidCredentials = "نام کاربری یا رمز عبور اشتباه است";
    public const string AccountLocked = "حساب کاربری قفل شده است. لطفاً {0} دقیقه دیگر تلاش کنید";
    public const string AccountInactive = "حساب کاربری غیرفعال است";
    public const string TokenExpired = "توکن منقضی شده است";
    public const string InvalidToken = "توکن نامعتبر است";

    // پیام‌های دسترسی
    public const string Unauthorized = "شما به این بخش دسترسی ندارید";
    public const string Forbidden = "این عملیات برای شما مجاز نیست";

    // پیام‌های منطق تجاری
    public const string InsufficientBalance = "موجودی کافی نیست";
    public const string ExceedsLimit = "مقدار درخواستی از حد مجاز بیشتر است";
    public const string OperationNotAllowed = "این عملیات مجاز نیست";
    public const string SystemRole = "نقش‌های سیستمی قابل تغییر نیستند";
    public const string SystemPermission = "دسترسی‌های سیستمی قابل تغییر نیستند";
}
