LendTech.Application
این لایه شامل Business Logic، CQRS Handlers، DTOs و Validators است.
ساختار پروژه
LendTech.Application/
├── Commands/         # Command ها و Handler ها
├── Queries/         # Query ها و Handler ها
├── DTOs/            # Data Transfer Objects
├── Validators/      # FluentValidation
├── Mappings/        # AutoMapper Profiles
├── Services/        # Business Services
├── Interfaces/      # Service Interfaces
├── Behaviors/       # MediatR Behaviors
└── Extensions/      # Extension Methods
ویژگی‌های کلیدی
CQRS Pattern

جداسازی Commands (تغییرات) از Queries (خواندن)
استفاده از MediatR برای مدیریت درخواست‌ها
بدون پیچیدگی‌های DDD

DTOs

جداسازی کامل از Entity ها
DTOs مخصوص برای Create, Update, List
استفاده از AutoMapper برای نگاشت

Validation

FluentValidation برای اعتبارسنجی
ValidationBehavior برای اعتبارسنجی خودکار
پیام‌های خطای فارسی

Business Services

ITokenService: مدیریت JWT
IPermissionService: مدیریت دسترسی‌ها

MediatR Behaviors

ValidationBehavior: اعتبارسنجی خودکار
LoggingBehavior: لاگ‌گیری درخواست‌ها

نحوه استفاده
csharp// ارسال Command
var command = new CreateUserCommand
{
    OrganizationId = organizationId,
    User = createUserDto
};
var result = await _mediator.Send(command);

// ارسال Query
var query = new GetUsersListQuery
{
    OrganizationId = organizationId,
    PagedRequest = pagedRequestDto
};
var result = await _mediator.Send(query);
وابستگی‌ها

MediatR 12.2.0
AutoMapper 13.0.1
FluentValidation 11.9.0
Ardalis.Result 7.2.0
