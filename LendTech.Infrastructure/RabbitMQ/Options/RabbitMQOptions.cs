namespace LendTech.Infrastructure.RabbitMQ.Options;
/// <summary>
/// تنظیمات RabbitMQ
/// </summary>
public class RabbitMQOptions
{
/// <summary>
/// آدرس هاست
/// </summary>
public string HostName { get; set; } = "localhost";
/// <summary>
/// پورت
/// </summary>
public int Port { get; set; } = 5672;

/// <summary>
/// نام کاربری
/// </summary>
public string UserName { get; set; } = "guest";

/// <summary>
/// رمز عبور
/// </summary>
public string Password { get; set; } = "guest";

/// <summary>
/// Virtual Host
/// </summary>
public string VirtualHost { get; set; } = "/";

/// <summary>
/// تعداد پیام همزمان
/// </summary>
public ushort PrefetchCount { get; set; } = 10;

/// <summary>
/// زمان انقضای پیام (میلی‌ثانیه)
/// </summary>
public int MessageTTL { get; set; } = 86400000; // 24 ساعت
}
