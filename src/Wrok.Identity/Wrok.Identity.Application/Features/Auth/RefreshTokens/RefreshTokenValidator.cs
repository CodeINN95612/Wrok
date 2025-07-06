using FluentValidation;

namespace Wrok.Identity.Application.Features.Auth.RefreshTokens;

internal sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}
