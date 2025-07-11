using FluentValidation;

namespace Wrok.Identity.Application.Features.Auth.RefreshTokens;

internal sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithErrorCode(RefreshTokenErrors.RefreshTokenRequired.Code)
            .WithMessage(RefreshTokenErrors.RefreshTokenRequired.Message);
    }
}
