using FluentValidation;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Features.Auth.Login;

internal class LoginRequestValidator: AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(IRegexValidator regexValidator)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode(LoginErrors.EmailRequired.Code)
            .WithMessage(LoginErrors.EmailRequired.Message);

        RuleFor(x => x.Email)
            .Must(regexValidator.IsValidEmail)
            .WithErrorCode(LoginErrors.EmailInvalid.Code)
            .WithMessage(LoginErrors.EmailInvalid.Message);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(LoginErrors.PasswordRequired.Code)
            .WithMessage(LoginErrors.PasswordRequired.Message);
    }
}
