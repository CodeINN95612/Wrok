using FluentValidation;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Features.Auth.Register;

internal sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(IRegexValidator regexValidator)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(regexValidator.IsValidEmail);
        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(regexValidator.IsValidPassword);
        RuleFor(x => x.FullName)
            .NotEmpty();
        RuleFor(x => x.TenantName)
            .NotEmpty();
    }
}
