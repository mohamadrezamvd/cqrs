using FluentValidation;
using LendTech.Application.DTOs.Auth;
using LendTech.SharedKernel.Constants;
namespace LendTech.Application.Validators.Auth;
/// <summary>
/// اعتبارسنجی درخواست ورود
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
public LoginRequestValidator()
{
RuleFor(x => x.Username)
.NotEmpty().WithMessage(ValidationMessages.Required.Replace("{0}", "نام کاربری"));
    RuleFor(x => x.Password)
        .NotEmpty().WithMessage(ValidationMessages.Required.Replace("{0}", "رمز عبور"));

    RuleFor(x => x.OrganizationId)
        .NotEmpty().WithMessage(ValidationMessages.Required.Replace("{0}", "سازمان"));
}
}
