LendTech.Infrastructure
این لایه شامل پیاده‌سازی Repository Pattern، Cache، Messaging و Security Services است.
ساختار پروژه
LendTech.Infrastructure/
├── Repositories/       # پیاده‌سازی Repository Pattern
│   ├── Interfaces/    # اینترفیس‌های Repository
│   └── *.cs          # پیاده‌سازی Repository ها
├── Redis/            # Cache Service با Redis
│   ├── Interfaces/   # ICacheService
│   └── CacheService.cs
├── RabbitMQ/         # Messaging با RabbitMQ
│   ├── Interfaces/   # IMessagePublisher, IMessageConsumer
│   ├── Options/      # RabbitMQOptions
│   ├── MessagePublisher.cs
│   └── MessageConsumer.cs
├── Security/         # سرویس‌های امنیتی
│   ├── TokenService.cs      # JWT Token Management
│   └── PermissionService.cs # Permission Management
└── Extensions/       # Extension Methods
ویژگی‌های کلیدی
Repository Pattern

پیاده‌سازی Generic Repository
Repository های اختصاصی برای هر Entity
بدون Unit of Work
پشتیبانی از Soft Delete

Cache Service

پیاده‌سازی با Redis
متدهای Generic برای Get/Set/Remove
GetOrSet Pattern
مدیریت Expiration

RabbitMQ Integration

Publisher برای ارسال پیام‌ها
Consumer برای دریافت پیام‌ها
Dead Letter Queue Support
Auto Recovery

Security Services

TokenService: مدیریت JWT
PermissionService: مدیریت دسترسی‌ها
Token Revocation با Redis
Permission Caching

تنظیمات (appsettings.json)
json{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LendTech;...",
    "Redis": "localhost:6379"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "PrefetchCount": 10,
    "MessageTTL": 86400000
  },
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "LendTech",
    "Audience": "LendTechApp",
    "ExpirationHours": 24
  }
}
وابستگی‌ها

Entity Framework Core 9.0.0
StackExchange.Redis 9.0.0
RabbitMQ.Client 6.8.1
System.IdentityModel.Tokens.Jwt 7.3.1
Polly 8.3.0
