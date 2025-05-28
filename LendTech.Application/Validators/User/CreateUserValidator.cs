using FluentValidation;
using LendTech.Application.DTOs.User;
using LendTech.SharedKernel.Constants;
using LendTech.SharedKernel.Extensions;
namespace LendTech.Application.Validators.User;
/// <summary>
/// اعتبارسنجی ایجاد کاربر
/// </summary>
public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
public CreateUserValidator()
{
RuleFor(x => x.Username)
.NotEmpty().WithMessage(ValidationMessages.Required.Replace("{0}", "نام کاربری"))
.MinimumLength(3).WithMessage(ValidationMessages.MinLength.Replace("{0}", "نام کاربری").Replace("{1}", "3"))
.MaximumLength(100).WithMessage(ValidationMessages.MaxLength.Replace("{0}", "نام کاربری").Replace("{1}", "100"))
.Matches("^[a-zA-Z0-9._]+$").WithMessage("نام کاربری فقط می‌تواند شامل حروف، اعداد، نقطه و آندرلاین باشد");
    RuleFor(x => x.Email)
        .NotEmpty().WithMessage(ValidationMessages.Required.Replace("{0}", "ایمیل"))
        .EmailAddress().WithMessage(ValidationMessages.Email)
        .MaximumLength(256).WithMessage(ValidationMessages.MaxLength.Replace("{0}", "ایمیل").Replace("{1}", "256"));

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage(ValidationMessages.Required.Replace("{0}", "رمز عبور"))
        .MinimumLength(SecurityConstants.MinPasswordLength)
            .WithMessage(ValidationMessages.PasswordTooShort.Replace("{0}", SecurityConstants.MinPasswordLength.ToString()));

    When(x => !string.IsNullOrEmpty(x.Password), () => {
        RuleFor(x => x.Password)
            .Must(password => SecurityConstants.RequireUppercase ? password.Any(char.IsUpper) : true)
                .WithMessage(ValidationMessages.PasswordRequiresUppercase)
            .Must(password => SecurityConstants.RequireLowercase ? password.Any(char.IsLower) : true)
                .WithMessage(ValidationMessages.PasswordRequiresLowercase)
            .Must(password => SecurityConstants.RequireDigit ? password.Any(char.IsDigit) : true)
                .WithMessage(ValidationMessages.PasswordRequiresDigit)
            .Must(password => SecurityConstants.RequireSpecialChar ? password.Any(c => !char.IsLetterOrDigit(c)) : true)
                .WithMessage(ValidationMessages.PasswordRequiresSpecialChar);
    });

    RuleFor(x => x.FirstName)
        .MaximumLength(100).WithMessage(ValidationMessages.MaxLength.Replace("{0}", "نام").Replace("{1}", "100"));

    RuleFor(x => x.LastName)
        .MaximumLength(100).WithMessage(ValidationMessages.MaxLength.Replace("{0}", "نام خانوادگی").Replace("{1}", "100"));

    RuleFor(x => x.MobileNumber)
        .Must(mobile => string.IsNullOrEmpty(mobile) || mobile.IsValidIranianMobile())
        .WithMessage(ValidationMessages.Phone)
        .MaximumLength(20).WithMessage(ValidationMessages.MaxLength.Replace("{0}", "شماره موبایل").Replace("{1}", "20"));
}
}
