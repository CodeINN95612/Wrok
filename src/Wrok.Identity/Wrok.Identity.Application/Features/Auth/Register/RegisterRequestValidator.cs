using FluentValidation;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Features.Auth.Register;

internal sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(IRegexValidator regexValidator)
    {
        var requiredEmail = RegisterErrors.EmailRequired;
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode(requiredEmail.Code)
            .WithMessage(requiredEmail.Message);

        var invalidEmail = RegisterErrors.EmailInvalid;
        RuleFor(x => x.Email)
            .Must(regexValidator.IsValidEmail)
            .WithErrorCode(invalidEmail.Code)
            .WithMessage(invalidEmail.Message);

        var requiredPassword = RegisterErrors.PasswordRequired;
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(requiredPassword.Code)
            .WithMessage(requiredPassword.Message);

        var invalidPassword = RegisterErrors.PasswordInvalid;
        RuleFor(x => x.Password)
            .Must(regexValidator.IsValidPassword)
            .WithErrorCode(invalidPassword.Code)
            .WithMessage(invalidPassword.Message);

        var requiredFullName = RegisterErrors.FullNameRequired;
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithErrorCode(requiredFullName.Code)
            .WithMessage(requiredFullName.Message);

        var requiredTenantName = RegisterErrors.TenantNameRequired;
        RuleFor(x => x.TenantName)
            .NotEmpty()
            .WithErrorCode(requiredTenantName.Code)
            .WithMessage(requiredTenantName.Message);
    }
}
